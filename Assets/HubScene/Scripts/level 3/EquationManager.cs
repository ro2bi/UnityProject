using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class LevelStage
{
    public string levelName;
    public LevelData levelData;
    public GameObject levelContainer;
    public EquationSlot[] levelSlots; // Убедись, что тут ВСЕ 5 слотов для КАЖДОГО уровня
}

public class EquationManager : MonoBehaviour
{
    [Header("Main Settings")]
    public List<LevelStage> stages;
    public int currentStageIndex = 0;
    public CinematicManager cinematicManager;
    public GameObject finalWall;

    [Header("Visual Goal")]
    public TMP_Text goalTextMesh;

    [Header("Timer Settings")]
    public float timeLimit = 30f;
    public Slider timerUI;
    private float currentTimer;
    private bool timerIsActive = false;

    private int totalErrors = 0;
    private bool isProcessing = false;
    private List<WeightObject> itemsOnCurrentLevel = new List<WeightObject>();

    void Start() => SetupCurrentStage();

    void Update()
    {
        if (timerIsActive && !isProcessing)
        {
            currentTimer -= Time.deltaTime;
            if (timerUI != null) timerUI.value = currentTimer / timeLimit;
            if (currentTimer <= 0) OnTimeOut();
        }
    }

    public void SetupCurrentStage()
    {
        totalErrors = 0;
        isProcessing = false;
        timerIsActive = false; // Выключаем до старта
        currentTimer = timeLimit;

        if (timerUI != null)
        {
            timerUI.gameObject.SetActive(false);
            timerUI.value = 1f;
        }

        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];

        if (goalTextMesh != null && current.levelData != null)
            goalTextMesh.text = current.levelData.targetResult.ToString();

        // Важно: обновляем список предметов именно этого контейнера
        itemsOnCurrentLevel.Clear();
        if (current.levelContainer != null)
        {
            current.levelContainer.SetActive(true);
            itemsOnCurrentLevel.AddRange(current.levelContainer.GetComponentsInChildren<WeightObject>());
        }

        ResetCurrentLevelItems();

        if (current.levelData != null)
            cinematicManager.PlayIntro(current.levelData);
    }

    public void StartLevelTimer()
    {
        if (!timerIsActive)
        {
            timerIsActive = true;
            if (timerUI != null) timerUI.gameObject.SetActive(true);
            Debug.Log("Таймер запущен!");
        }
    }

    public void CheckFullEquation()
    {
        if (isProcessing) return;

        LevelStage current = stages[currentStageIndex];

        // ДЛЯ ОТЛАДКИ: Если не считает, посмотри в консоль, сколько слотов видит скрипт
        int occupiedCount = 0;
        foreach (var slot in current.levelSlots)
        {
            if (slot != null && slot.isOccupied) occupiedCount++;
        }

        Debug.Log($"Проверка: {occupiedCount} из {current.levelSlots.Length} слотов заполнены.");

        // Проверяем, все ли слоты, указанные в инспекторе для ЭТОГО уровня, заполнены
        foreach (var slot in current.levelSlots)
        {
            if (slot == null || !slot.isOccupied) return;
        }

        StartCoroutine(ProcessResultRoutine());
    }

    private IEnumerator ProcessResultRoutine()
    {
        isProcessing = true;
        LevelStage current = stages[currentStageIndex];

        string expression = "";
        foreach (var slot in current.levelSlots) expression += slot.GetValueAsString();

        Debug.Log("Вычисляем выражение: " + expression);

        double result = EvaluateExpression(expression);
        bool isCorrect = Mathf.Approximately((float)result, (float)current.levelData.targetResult);

        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            timerIsActive = false;
            if (timerUI != null) timerUI.gameObject.SetActive(false);

            ShuffleItemsPositions();

            bool isLast = (currentStageIndex == stages.Count - 1);
            cinematicManager.PlayOutcome(current.levelData.winVideo, true, isLast, totalErrors);
        }
        else
        {
            totalErrors++;
            ResetCurrentLevelItems();
            isProcessing = false;
        }
    }

    private double EvaluateExpression(string expr)
    {
        try { return System.Convert.ToDouble(new DataTable().Compute(expr.Replace(",", "."), "")); }
        catch { return -99999; }
    }

    private void OnTimeOut()
    {
        ShuffleItemsPositions();
        currentTimer = timeLimit;
    }

    private void ShuffleItemsPositions()
    {
        FindObjectOfType<PlayerMovementNew>()?.ForceDrop();
        LevelStage current = stages[currentStageIndex];

        foreach (var slot in current.levelSlots)
        {
            slot.GetItem()?.ReturnToStart();
            slot.ResetSlotManually();
        }

        if (itemsOnCurrentLevel.Count < 2) return;

        Vector3[] positions = new Vector3[itemsOnCurrentLevel.Count];
        for (int i = 0; i < itemsOnCurrentLevel.Count; i++)
            positions[i] = itemsOnCurrentLevel[i].startPosition;

        for (int i = 0; i < itemsOnCurrentLevel.Count; i++)
        {
            int nextIndex = (i + 1) % itemsOnCurrentLevel.Count;
            itemsOnCurrentLevel[i].StopAllCoroutines();
            itemsOnCurrentLevel[i].startPosition = positions[nextIndex];
            itemsOnCurrentLevel[i].StartCoroutine(itemsOnCurrentLevel[i].MoveToPos(positions[nextIndex]));
        }
    }

    private void ResetCurrentLevelItems()
    {
        if (currentStageIndex >= stages.Count) return;
        foreach (var slot in stages[currentStageIndex].levelSlots)
        {
            slot.GetItem()?.ReturnToStart();
            slot.ResetSlotManually();
        }
    }

    public void GoToNextLevel()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            SetupCurrentStage();

            // ИСПРАВЛЕНИЕ: Автоматически запускаем таймер на следующем уровне
            StartLevelTimer();
        }
    }

    public void OpenWallAndFinish()
    {
        if (finalWall != null) finalWall.SetActive(false);
    }
}