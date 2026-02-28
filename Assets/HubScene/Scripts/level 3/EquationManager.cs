using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data; // Используется для вычисления математических строк
using UnityEngine.Video;
using UnityEngine.UI; // Для работы со Slider

// Структура для настройки уровней в инспекторе
[System.Serializable]
public class LevelStage
{
    public string levelName;
    public LevelData levelData;
    public GameObject levelContainer;
    public EquationSlot[] levelSlots; // Массив слотов (теперь их может быть 5: Число-Знак-Число-Знак-Число)
}

public class EquationManager : MonoBehaviour
{
    private List<WeightObject> itemsOnCurrentLevel = new List<WeightObject>();

    [Header("Main Settings")]
    public List<LevelStage> stages;
    public int currentStageIndex = 0;
    public CinematicManager cinematicManager;
    public GameObject finalWall;

    [Header("Timer Settings")]
    public float timeLimit = 30f;        // Лимит времени на уровень
    public Slider timerUI;               // Ссылка на UI Slider
    private float currentTimer;
    private bool timerIsActive = false;  // Флаг активности таймера

    private int totalErrors = 0;
    private bool isProcessing = false;   // Чтобы не считать дважды во время анимации

    void Start()
    {
        SetupCurrentStage();
    }

    void Update()
    {
        // Логика отсчета таймера
        if (timerIsActive && !isProcessing)
        {
            currentTimer -= Time.deltaTime;

            // Обновляем визуальный слайдер
            if (timerUI != null)
            {
                timerUI.value = currentTimer / timeLimit;
            }

            // Если время вышло
            if (currentTimer <= 0)
            {
                OnTimeOut();
            }
        }
    }

    public void SetupCurrentStage()
    {
        totalErrors = 0;
        isProcessing = false;
        timerIsActive = false;
        currentTimer = timeLimit;

        if (timerUI != null)
        {
            timerUI.gameObject.SetActive(false);
            timerUI.value = 1f;
        }

        for (int i = 0; i < stages.Count; i++)
        {
            bool isActive = (i == currentStageIndex);
            if (stages[i].levelContainer != null)
            {
                stages[i].levelContainer.SetActive(isActive);

                // Если это текущий уровень — запоминаем ВСЕ его предметы заранее
                if (isActive)
                {
                    itemsOnCurrentLevel.Clear();
                    itemsOnCurrentLevel.AddRange(stages[i].levelContainer.GetComponentsInChildren<WeightObject>());
                }
            }
        }

        if (stages[currentStageIndex].levelData != null)
        {
            cinematicManager.PlayIntro(stages[currentStageIndex].levelData);
        }
    }

    // Вызывается из скрипта LevelStartTrigger при входе игрока в зону
    public void StartLevelTimer()
    {
        if (!timerIsActive)
        {
            timerIsActive = true;
            currentTimer = timeLimit;

            // Показываем слайдер
            if (timerUI != null)
            {
                timerUI.gameObject.SetActive(true);
            }
            Debug.Log("Таймер запущен через триггер!");
        }
    }

    // Проверка заполненности всех клеток
    public void CheckFullEquation()
    {
        if (isProcessing) return;

        LevelStage current = stages[currentStageIndex];

        // Проверяем, все ли слоты заняты
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
        LevelData data = current.levelData;

        // 1. Собираем строку уравнения из всех слотов
        string expression = "";
        foreach (var slot in current.levelSlots)
        {
            expression += slot.GetValueAsString();
        }

        // 2. Вычисляем результат через DataTable
        double result = EvaluateExpression(expression);
        bool isCorrect = Mathf.Approximately((float)result, (float)data.targetResult);

        // 3. Показываем визуальный фидбек (зеленый/красный)
        foreach (var slot in current.levelSlots) slot.SetFeedback(isCorrect);

        yield return new WaitForSeconds(1.2f);

        if (isCorrect)
        {
            timerIsActive = false;
            // Прячем слайдер при победе
            if (timerUI != null) timerUI.gameObject.SetActive(false);

            bool isLast = (currentStageIndex == stages.Count - 1);
            cinematicManager.PlayOutcome(data.winVideo, true, isLast, totalErrors);
        }
        else
        {
            totalErrors++;
            VideoClip failVideo = (result < data.targetResult) ? data.tooLowVideo : data.tooHighVideo;
            cinematicManager.PlayOutcome(failVideo, false, false);

            yield return new WaitForSeconds(0.5f);
            ResetCurrentLevelItems(); // Сбрасываем предметы при ошибке
            isProcessing = false;
        }
    }

    // Математический движок (поддерживает +, -, *, /)
    private double EvaluateExpression(string expression)
    {
        try
        {
            // Обработка точки/запятой для универсальности
            expression = expression.Replace(",", ".");
            DataTable table = new DataTable();
            var computed = table.Compute(expression, "");
            return System.Convert.ToDouble(computed);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка в формуле: " + expression + " | " + e.Message);
            return -999999;
        }
    }

    // Событие окончания времени
    private void OnTimeOut()
    {
        Debug.Log("Время вышло! Перемешивание...");
        ShuffleItemsPositions();
        currentTimer = timeLimit; // Даем новую попытку
    }

    private void ShuffleItemsPositions()
    {
        LevelStage current = stages[currentStageIndex];

        // 1. Заставляем игрока бросить предмет, если он его держит
        PlayerMovementNew player = FindObjectOfType<PlayerMovementNew>();
        if (player != null) player.ForceDrop(); // Метод добавим ниже

        // 2. Очищаем слоты
        foreach (var slot in current.levelSlots)
        {
            WeightObject itemInSlot = slot.GetItem();
            if (itemInSlot != null) itemInSlot.ReturnToStart();
            slot.ResetSlotManually();
        }

        // 3. ТЕПЕРЬ МЫ ИСПОЛЬЗУЕМ ЗАРАНЕЕ СОЗДАННЫЙ СПИСОК
        if (itemsOnCurrentLevel.Count < 2) return;

        // Запоминаем текущие стартовые позиции всех предметов из нашего списка
        Vector3[] positions = new Vector3[itemsOnCurrentLevel.Count];
        for (int i = 0; i < itemsOnCurrentLevel.Count; i++)
        {
            positions[i] = itemsOnCurrentLevel[i].startPosition;
        }

        // Перемещаем предметы по кругу
        for (int i = 0; i < itemsOnCurrentLevel.Count; i++)
        {
            int nextIndex = (i + 1) % itemsOnCurrentLevel.Count;
            itemsOnCurrentLevel[i].StopAllCoroutines();
            itemsOnCurrentLevel[i].StartCoroutine(itemsOnCurrentLevel[i].MoveToPos(positions[nextIndex]));
        }
    }

    // Сброс предметов текущего уровня вручную
    private void ResetCurrentLevelItems()
    {
        foreach (var slot in stages[currentStageIndex].levelSlots)
        {
            WeightObject item = slot.GetItem();
            if (item != null) item.ReturnToStart();
            slot.ResetSlotManually();
        }
    }

    public void GoToNextLevel()
    {
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            SetupCurrentStage();
        }
    }

    public void OpenWallAndFinish()
    {
        if (finalWall != null) finalWall.SetActive(false);
    }
}