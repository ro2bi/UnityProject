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

    [Header("Slots")]
    public EquationSlot[] levelSlots;
    public EquationSlot[] rightSideSlots;

    [Header("Optional: Result Tablets")]
    public TMP_Text leftSideDisplay;
    public TMP_Text rightSideDisplay;

    [Header("Level Rewards")]
    public GameObject wallToDeactivate; // ТА САМАЯ СТЕНА для конкретного уровня
}

public class EquationManager : MonoBehaviour
{
    [Header("Main Settings")]
    public List<LevelStage> stages;
    public int currentStageIndex = 0;
    public CinematicManager cinematicManager;
    public GameObject finalWall; // Общая стена (если нужна в конце игры)

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
        timerIsActive = false;
        currentTimer = timeLimit;

        if (timerUI != null) { timerUI.gameObject.SetActive(false); timerUI.value = 1f; }

        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];

        if (goalTextMesh != null && current.levelData != null)
        {
            bool isComparisonMode = current.rightSideSlots != null && current.rightSideSlots.Length > 0;
            goalTextMesh.text = isComparisonMode ? "=" : current.levelData.targetResult.ToString();
        }

        itemsOnCurrentLevel.Clear();
        if (current.levelContainer != null)
        {
            current.levelContainer.SetActive(true);
            itemsOnCurrentLevel.AddRange(current.levelContainer.GetComponentsInChildren<WeightObject>());
        }

        ResetCurrentLevelItems();
        UpdateTablets();
        if (current.levelData != null) cinematicManager.PlayIntro(current.levelData);
    }

    public void UpdateTablets()
    {
        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];

        if (current.leftSideDisplay != null)
        {
            string expr = "";
            foreach (var s in current.levelSlots) expr += s.GetValueAsString();
            double res = EvaluateExpression(expr);
            current.leftSideDisplay.text = (res == -99999 || expr == "") ? "?" : res.ToString();
        }

        if (current.rightSideDisplay != null)
        {
            string expr = "";
            foreach (var s in current.rightSideSlots) expr += s.GetValueAsString();
            double res = EvaluateExpression(expr);
            current.rightSideDisplay.text = (res == -99999 || expr == "") ? "?" : res.ToString();
        }
    }

    public void CheckFullEquation()
    {
        UpdateTablets();
        if (isProcessing) return;
        LevelStage current = stages[currentStageIndex];

        foreach (var slot in current.levelSlots) if (slot == null || !slot.isOccupied) return;
        if (current.rightSideSlots != null && current.rightSideSlots.Length > 0)
        {
            foreach (var slot in current.rightSideSlots) if (slot == null || !slot.isOccupied) return;
        }

        StartCoroutine(ProcessResultRoutine());
    }

    private IEnumerator ProcessResultRoutine()
    {
        isProcessing = true;
        LevelStage current = stages[currentStageIndex];

        string leftExpr = "";
        foreach (var slot in current.levelSlots) leftExpr += slot.GetValueAsString();
        double leftVal = EvaluateExpression(leftExpr);

        bool isCorrect = false;

        if (current.rightSideSlots != null && current.rightSideSlots.Length > 0)
        {
            string rightExpr = "";
            foreach (var slot in current.rightSideSlots) rightExpr += slot.GetValueAsString();
            double rightVal = EvaluateExpression(rightExpr);
            isCorrect = (leftVal != -99999 && rightVal != -99999) && Mathf.Approximately((float)leftVal, (float)rightVal);
        }
        else
        {
            isCorrect = Mathf.Approximately((float)leftVal, (float)current.levelData.targetResult);
        }

        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);
        if (current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            timerIsActive = false;
            if (timerUI != null) timerUI.gameObject.SetActive(false);

            // НОВОЕ: Отключаем стену ЭТОГО уровня
            if (current.wallToDeactivate != null)
            {
                current.wallToDeactivate.SetActive(false);
            }

            bool isLast = (currentStageIndex == stages.Count - 1);
            cinematicManager.PlayOutcome(current.levelData.winVideo, true, isLast, totalErrors);
        }
        else
        {
            totalErrors++;
            ResetCurrentLevelItems();
            UpdateTablets();
            isProcessing = false;
        }
    }

    private double EvaluateExpression(string expr)
    {
        try { return System.Convert.ToDouble(new DataTable().Compute(expr.Replace(",", "."), "")); }
        catch { return -99999; }
    }

    private void OnTimeOut() { ShuffleItemsPositions(); UpdateTablets(); currentTimer = timeLimit; }

    private void ShuffleItemsPositions()
    {
        FindObjectOfType<PlayerMovementNew>()?.ForceDrop();
        LevelStage current = stages[currentStageIndex];
        foreach (var slot in current.levelSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
        if (current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }

        if (itemsOnCurrentLevel.Count < 2) return;
        foreach (var item in itemsOnCurrentLevel) item.ReturnToStart();
    }

    private void ResetCurrentLevelItems()
    {
        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];
        foreach (var slot in current.levelSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
        if (current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
    }

    public void StartLevelTimer() { timerIsActive = true; if (timerUI != null) timerUI.gameObject.SetActive(true); }
    public void GoToNextLevel() { if (currentStageIndex < stages.Count - 1) { currentStageIndex++; SetupCurrentStage(); StartLevelTimer(); } }

    // Этот метод теперь можно использовать для финальной стены в самом конце игры
    public void OpenWallAndFinish() { if (finalWall != null) finalWall.SetActive(false); }
}