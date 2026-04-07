using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

[System.Serializable]
public class LevelStage
{
    public string levelName;
    public LevelData levelData;
    public GameObject levelContainer;

    [Header("Настройки Режима")]
    public bool isComparisonMode = false;
    [Tooltip("Если включено, то ПОСЛЕ завершения этого уровня таймер следующего уровня сам не включится")]
    public bool stopTimerAfterThisLevel = false;

    [Header("Слоты")]
    public EquationSlot[] levelSlots;
    public EquationSlot[] rightSideSlots;

    [Header("Таблички")]
    public TMP_Text leftSideDisplay;
    public TMP_Text rightSideDisplay;

    [Header("Награда")]
    public GameObject wallToDeactivate;
}

public class EquationManager : MonoBehaviour
{
    [Header("Main Settings")]
    public List<LevelStage> stages;
    public int currentStageIndex = 0;
    public CinematicManager cinematicManager;
    public GameObject finalWall;

    [Header("UI элементы")]
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

        if (goalTextMesh != null)
        {
            if (current.isComparisonMode) goalTextMesh.text = "=";
            else if (current.levelData != null) goalTextMesh.text = current.levelData.targetResult.ToString();
        }

        itemsOnCurrentLevel.Clear();
        if (current.levelContainer != null)
        {
            current.levelContainer.SetActive(true);
            itemsOnCurrentLevel.AddRange(current.levelContainer.GetComponentsInChildren<WeightObject>());
        }

        ResetCurrentLevelItems();
        UpdateTablets();
        if (current.levelData != null && cinematicManager != null) cinematicManager.PlayIntro(current.levelData);
    }

    public void GoToNextLevel()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            bool skipNextTimer = stages[currentStageIndex].stopTimerAfterThisLevel;

            currentStageIndex++;
            SetupCurrentStage();

            if (!skipNextTimer)
            {
                StartLevelTimer();
            }
            else
            {
                Debug.Log($"[ТАЙМЕР] Автозапуск заблокирован после уровня: {stages[currentStageIndex - 1].levelName}");
            }
        }
    }

    private void LogSlotsProgress()
    {
        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];
        int occupiedCount = 0;
        int totalCount = current.levelSlots.Length + (current.isComparisonMode ? current.rightSideSlots.Length : 0);

        foreach (var s in current.levelSlots) if (s != null && s.isOccupied) occupiedCount++;
        if (current.isComparisonMode && current.rightSideSlots != null)
            foreach (var s in current.rightSideSlots) if (s != null && s.isOccupied) occupiedCount++;

        Debug.Log($"[ПРОГРЕСС] Уровень: {current.levelName} | Заполнено: {occupiedCount} / {totalCount}");
    }

    public void UpdateTablets()
    {
        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];
        LogSlotsProgress();

        if (current.leftSideDisplay != null)
        {
            string expr = BuildExpression(current.levelSlots);
            double res = EvaluateExpression(expr);
            current.leftSideDisplay.text = (res == -99999) ? "?" : res.ToString();
        }

        if (current.rightSideDisplay != null)
        {
            if (current.isComparisonMode)
            {
                string expr = BuildExpression(current.rightSideSlots);
                double res = EvaluateExpression(expr);
                current.rightSideDisplay.text = (res == -99999) ? "?" : res.ToString();
            }
            else if (current.levelData != null)
            {
                current.rightSideDisplay.text = current.levelData.targetResult.ToString();
            }
        }
    }

    private string BuildExpression(EquationSlot[] slots)
    {
        string fullExpr = "";
        foreach (var s in slots)
        {
            if (s != null && s.isOccupied) fullExpr += s.GetValueAsString();
            else fullExpr += " ";
        }
        return fullExpr.Trim();
    }

    public void CheckFullEquation()
    {
        UpdateTablets();
        if (isProcessing) return;
        LevelStage current = stages[currentStageIndex];

        foreach (var slot in current.levelSlots) if (slot == null || !slot.isOccupied) return;

        if (current.isComparisonMode)
        {
            if (current.rightSideSlots == null || current.rightSideSlots.Length == 0) return;
            foreach (var slot in current.rightSideSlots) if (slot == null || !slot.isOccupied) return;
        }

        StartCoroutine(ProcessResultRoutine());
    }

    private IEnumerator ProcessResultRoutine()
    {
        isProcessing = true;
        LevelStage current = stages[currentStageIndex];

        string leftExpr = BuildExpression(current.levelSlots);
        double leftVal = EvaluateExpression(leftExpr);
        bool isCorrect = false;

        if (current.isComparisonMode)
        {
            string rightExpr = BuildExpression(current.rightSideSlots);
            double rightVal = EvaluateExpression(rightExpr);
            isCorrect = (leftVal != -99999 && rightVal != -99999) && Mathf.Approximately((float)leftVal, (float)rightVal);
        }
        else if (current.levelData != null)
        {
            isCorrect = (leftVal != -99999) && Mathf.Approximately((float)leftVal, (float)current.levelData.targetResult);
        }

        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);
        if (current.isComparisonMode && current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            timerIsActive = false;
            if (timerUI != null) timerUI.gameObject.SetActive(false);
            if (current.wallToDeactivate != null) current.wallToDeactivate.SetActive(false);

            bool isLast = (currentStageIndex == stages.Count - 1);
            if (current.levelData != null && cinematicManager != null)
                cinematicManager.PlayOutcome(current.levelData.winVideo, true, isLast, totalErrors);
            else
            {
                if (isLast) OpenWallAndFinish(); else GoToNextLevel();
            }
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
        if (string.IsNullOrWhiteSpace(expr)) return -99999;
        try
        {
            var table = new DataTable();
            string cleanExpr = expr.Replace(",", ".");
            object result = table.Compute(cleanExpr, "");
            return System.Convert.ToDouble(result, CultureInfo.InvariantCulture);
        }
        catch { return -99999; }
    }

    private void OnTimeOut() { ShuffleItemsPositions(); UpdateTablets(); currentTimer = timeLimit; }

    private void ShuffleItemsPositions()
    {
        FindObjectOfType<PlayerMovementNew>()?.ForceDrop();
        LevelStage current = stages[currentStageIndex];
        foreach (var slot in current.levelSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
        if (current.isComparisonMode && current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }

        if (itemsOnCurrentLevel.Count < 2) return;
        foreach (var item in itemsOnCurrentLevel) item.ReturnToStart();
    }

    private void ResetCurrentLevelItems()
    {
        if (currentStageIndex >= stages.Count) return;
        LevelStage current = stages[currentStageIndex];
        foreach (var slot in current.levelSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
        if (current.isComparisonMode && current.rightSideSlots != null)
            foreach (var slot in current.rightSideSlots) { slot.GetItem()?.ReturnToStart(); slot.ResetSlotManually(); }
    }

    public void StartLevelTimer() { timerIsActive = true; if (timerUI != null) timerUI.gameObject.SetActive(true); }
    public void OpenWallAndFinish() { if (finalWall != null) finalWall.SetActive(false); }
}