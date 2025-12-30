using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;

// Структура для настройки каждого этапа прямо в инспекторе
[System.Serializable]
public class LevelStage
{
    public string levelName;         // Название (для удобства в инспекторе)
    public LevelData levelData;      // Ассет с настройками (ответ, видео, формула)
    public GameObject levelContainer; // Объект-папка, где лежат числа и слоты этого уровня
    public EquationSlot[] levelSlots; // 3 слота именно этого уровня
}

public class EquationManager : MonoBehaviour
{
    [Header("Main Settings")]
    public List<LevelStage> stages;      // Список всех уровней
    public int currentStageIndex = 0;    // Текущий уровень
    public CinematicManager cinematicManager;
    public GameObject finalWall;         // Стена, которая исчезнет в конце

    private int totalErrors = 0;         // Счетчик ошибок текущего уровня
    private bool isProcessing = false;   // Флаг, чтобы не считать дважды

    void Start()
    {
        // При старте выключаем все уровни, кроме первого
        SetupCurrentStage();
    }

    // Настройка текущего этапа
    public void SetupCurrentStage()
    {
        totalErrors = 0;
        isProcessing = false;

        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].levelContainer != null)
                stages[i].levelContainer.SetActive(i == currentStageIndex);
        }

        // Если есть видео-интро для этого уровня — играем (необязательно)
        if (stages[currentStageIndex].levelData != null)
        {
            cinematicManager.PlayIntro(stages[currentStageIndex].levelData);
        }
    }

    // Этот метод вызывается из EquationSlot каждый раз, когда вставляют число
    public void CheckFullEquation()
    {
        if (isProcessing) return;

        LevelStage current = stages[currentStageIndex];

        // Проверяем, заполнены ли все слоты текущего уровня
        foreach (var slot in current.levelSlots)
        {
            if (slot == null || !slot.isOccupied) return;
        }

        // Если все заполнены — запускаем проверку
        StartCoroutine(ProcessResultRoutine());
    }

    private IEnumerator ProcessResultRoutine()
    {
        isProcessing = true;
        LevelStage current = stages[currentStageIndex];
        LevelData data = current.levelData;

        int a = current.levelSlots[0].currentValue;
        int b = current.levelSlots[1].currentValue;
        int c = current.levelSlots[2].currentValue;

        int result = CalculateByFormula(a, b, c, data.formulaType);
        bool isCorrect = (result == data.targetResult);

        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            // ПРОВЕРКА: последний ли это уровень в списке
            bool isLast = (currentStageIndex == stages.Count - 1);

            // Передаем флаг isLast в CinematicManager
            cinematicManager.PlayOutcome(data.winVideo, true, isLast, totalErrors);
        }
        else
        {
            totalErrors++;
            VideoClip failVideo = (result < data.targetResult) ? data.tooLowVideo : data.tooHighVideo;

            // При ошибке всегда передаем false (кнопка Quit должна быть видна)
            cinematicManager.PlayOutcome(failVideo, false, false);

            yield return new WaitForSeconds(0.5f);
            ResetCurrentLevelItems();
            isProcessing = false;
        }
    }

    // Математический движок
    private int CalculateByFormula(int a, int b, int c, EquationType type)
    {
        switch (type)
        {
            case EquationType.A_plus_B_minus_C: return a + b - c;
            case EquationType.A_minusminus_B_plus_C: return a + b + c;
            case EquationType.A_mul_B_plus_C: return (a * b) + c;
            case EquationType.A_del_scob1_B_min_C_scob2: return a / (b - c);
            case EquationType.A_mul_scob1_B_minus_C_scob2: return a * (b - c);
            default: return 0;
        }
    }

    // Вызывается кнопкой "Next Level" на экране WIN
    public void GoToNextLevel()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            SetupCurrentStage();
        }
        else
        {
            // Этот код сработает, когда нажмут "Далее" на 4-м уровне
            Debug.Log("4 Уровня пройдены!");
        }
    }

    // Вызывается кнопкой "Quit" на экране WIN
    public void OpenWallAndFinish()
    {
        if (finalWall != null)
        {
            finalWall.SetActive(false); // Убираем невидимую стену
            Debug.Log("Путь открыт, стена исчезла!");
        }
    }

    // Сброс предметов текущего уровня
    private void ResetCurrentLevelItems()
    {
        foreach (var slot in stages[currentStageIndex].levelSlots)
        {
            WeightObject item = slot.GetItem();
            if (item != null)
            {
                item.ReturnToStart(); // Метод в скрипте WeightObject
            }
            slot.ResetSlotManually(); // Метод в скрипте EquationSlot
        }
    }
}