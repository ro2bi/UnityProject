using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Data;

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
    [Header("Timer Settings")]
    public float timeLimit = 30f; // Время на уровень в секундах
    private float currentTimer;
    private bool timerIsActive = false;

    public UnityEngine.UI.Slider timerUI; 

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

    public void SetupCurrentStage()
    {
        totalErrors = 0;
        isProcessing = false;
        currentTimer = timeLimit;
        timerIsActive = false; // ТЕПЕРЬ ТУТ FALSE ПО УМОЛЧАНИЮ

        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].levelContainer != null)
                stages[i].levelContainer.SetActive(i == currentStageIndex);
        }

        if (stages[currentStageIndex].levelData != null)
        {
            cinematicManager.PlayIntro(stages[currentStageIndex].levelData);
        }
    }

    public void StartLevelTimer()
    {
        if (!timerIsActive)
        {
            timerIsActive = true;
            Debug.Log("Таймер запущен!");
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

        // Собираем выражение из всех слотов
        string expression = "";
        foreach (var slot in current.levelSlots)
        {
            string val = slot.GetValueAsString();
            if (string.IsNullOrEmpty(val)) continue; 
            expression += val;
        }

        if (string.IsNullOrEmpty(expression)) { isProcessing = false; yield break; }

        // Вычисляем результат
        double result = EvaluateExpression(expression);
        bool isCorrect = (Mathf.Approximately((float)result, (float)data.targetResult));

        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            bool isLast = (currentStageIndex == stages.Count - 1);
            cinematicManager.PlayOutcome(data.winVideo, true, isLast, totalErrors);
        }
        else
        {
            totalErrors++;
            // Для простоты оставим логику "Больше/Меньше", 
            // хотя со сложными знаками она станет менее предсказуемой
            VideoClip failVideo = (result < data.targetResult) ? data.tooLowVideo : data.tooHighVideo;
            cinematicManager.PlayOutcome(failVideo, false, false);

            yield return new WaitForSeconds(0.5f);
            ResetCurrentLevelItems();
            isProcessing = false;
        }
    }

    private double EvaluateExpression(string expression)
    {
        try
        {
            // Заменяем возможные запятые на точки для корректного расчета (на случай локализации)
            expression = expression.Replace(",", ".");

            DataTable table = new DataTable();
            var result = table.Compute(expression, "");
            return System.Convert.ToDouble(result);
        }
        catch (System.Exception e)
        {
            // Если игрок составил что-то странное (например "5++3"), выведет ошибку в консоль
            Debug.LogError("Ошибка в математическом выражении: " + expression + " | " + e.Message);
            return -999999;
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

    void Update()
    {
        if (timerIsActive && !isProcessing)
        {
            currentTimer -= Time.deltaTime;

            if (timerUI != null)
                timerUI.value = currentTimer / timeLimit;

            if (currentTimer <= 0)
            {
                OnTimeOut();
            }
        }
    }

    private void OnTimeOut()
    {
        Debug.Log("Время вышло! Предметы меняются местами.");
        ShuffleItemsPositions();
        currentTimer = timeLimit; // Сбрасываем таймер, чтобы дать еще попытку
    }

    private void ShuffleItemsPositions()
    {
        LevelStage current = stages[currentStageIndex];

        // 1. ОЧИЩАЕМ ВСЕ СЛОТЫ
        foreach (var slot in current.levelSlots)
        {
            if (slot != null)
            {
                // Если в слоте был предмет, мы его не удаляем из игры, 
                // а просто говорим ему вернуться на базу
                WeightObject item = slot.GetItem();
                if (item != null)
                {
                    item.ReturnToStart();
                }

                // ПРИНУДИТЕЛЬНЫЙ СБРОС СОСТОЯНИЯ КЛЕТКИ
                slot.ResetSlotManually();
            }
        }

        // 2. ПОЛУЧАЕМ ВСЕ ПРЕДМЕТЫ УРОВНЯ (которые должны перемешаться)
        WeightObject[] items = current.levelContainer.GetComponentsInChildren<WeightObject>();

        if (items.Length < 2) return;

        // Запоминаем текущие стартовые позиции всех предметов
        Vector3[] positions = new Vector3[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            positions[i] = items[i].startPosition;
        }

        // 3. ПЕРЕМЕЩАЕМ ПРЕДМЕТЫ НА НОВЫЕ МЕСТА (сдвиг по кругу)
        for (int i = 0; i < items.Length; i++)
        {
            int nextIndex = (i + 1) % items.Length;
            items[i].StopAllCoroutines();
            // Запускаем плавный переезд в новую точку
            items[i].StartCoroutine(items[i].MoveToPos(positions[nextIndex]));
        }

        Debug.Log("Таймер истек: слоты очищены, предметы переехали.");
    }
}