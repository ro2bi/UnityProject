using UnityEngine;
using UnityEngine.UI;

public class LevelStagesManager : MonoBehaviour
{
    [System.Serializable]
    public class EquationVariant
    {
        public GameObject visualObject;
        public bool isUpperTriggerCorrect;
    }

    [System.Serializable]
    public class StageData
    {
        public int stageIndex;
        public GameObject enterWalls;
        public GameObject closeBehindWalls;
        public Transform teleportAfterStage;

        [Header("Timer & Equations")]
        public ChoiceTrigger upperTrigger;
        public ChoiceTrigger lowerTrigger;
        public EquationVariant[] variants;
        public float timeLimit = 5f;
        [HideInInspector] public int currentVariantIndex = 0;
    }

    [Header("Player & Physics")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Transform loseTeleportPoint;

    [Header("UI Timer")]
    [SerializeField] private Slider timerSlider;
    [SerializeField] private GameObject sliderContainer;

    [Header("Stages")]
    [SerializeField] private StageData[] stages;

    private Vector3 startPosition;
    private int currentStage;
    private float timerValue;
    private bool isTimerActive;

    private void Start()
    {
        startPosition = player.position;
        if (sliderContainer != null) sliderContainer.SetActive(false);
        ResetLevel();
    }

    private void Update()
    {
        if (isTimerActive)
        {
            timerValue -= Time.deltaTime;

            if (timerSlider != null)
            {
                // Используем лимит времени текущего этапа
                float currentLimit = stages[currentStage].timeLimit;
                timerSlider.value = timerValue / currentLimit;
            }

            // Если время вышло - меняем вариант в ТЕКУЩЕМ этапе и сбрасываем круг
            if (timerValue <= 0)
            {
                SwitchToNextVariant(currentStage);
            }
        }
    }

    // Запускается один раз триггером в начале уровня
    public void StartLevelTimer()
    {
        currentStage = 0;
        if (sliderContainer != null) sliderContainer.SetActive(true);
        ResetTimerForStage(0);
        isTimerActive = true;
    }

    private void ResetTimerForStage(int stageIdx)
    {
        if (stageIdx >= stages.Length) return;
        timerValue = stages[stageIdx].timeLimit;
        if (timerSlider != null) timerSlider.value = 1f;
    }

    public void StopAndHideTimer()
    {
        isTimerActive = false;
        if (sliderContainer != null) sliderContainer.SetActive(false);
    }

    private void SwitchToNextVariant(int stageIndex)
    {
        StageData stage = GetStage(stageIndex);
        if (stage == null || stage.variants.Length <= 1) return;

        // Скрываем старый вариант, показываем следующий
        stage.variants[stage.currentVariantIndex].visualObject.SetActive(false);
        stage.currentVariantIndex = (stage.currentVariantIndex + 1) % stage.variants.Length;

        EquationVariant newVar = stage.variants[stage.currentVariantIndex];
        newVar.visualObject.SetActive(true);

        // Обновляем правильность путей
        stage.upperTrigger.SetCorrect(newVar.isUpperTriggerCorrect);
        stage.lowerTrigger.SetCorrect(!newVar.isUpperTriggerCorrect);

        // Начинаем новый круг таймера для этого же этапа
        ResetTimerForStage(stageIndex);
    }

    public void OnCorrectChoice(int stageIndex)
    {
        if (stageIndex != currentStage) return;

        SetCloseBehindWalls(stageIndex, true);

        StageData stage = GetStage(stageIndex);
        if (stage != null && stage.teleportAfterStage != null)
            Teleport(stage.teleportAfterStage.position);

        currentStage++;

        // ПРОВЕРКА: Если это был последний этап
        if (currentStage >= stages.Length)
        {
            StopAndHideTimer(); // ВЫКЛЮЧАЕМ таймер навсегда
            Debug.Log("Level Complete! Timer Off.");
        }
        else
        {
            // Если впереди еще есть примеры - открываем стены и сбрасываем таймер на новый этап
            SetEnterWalls(currentStage, false);
            ResetTimerForStage(currentStage);
            Debug.Log($"Stage {stageIndex} passed. Timer reset for Stage {currentStage}");
        }
    }

    public void OnWrongChoice()
    {
        StopAndHideTimer();
        Teleport(loseTeleportPoint.position);
        ResetLevel();
        Teleport(startPosition);
    }

    private void ResetLevel()
    {
        currentStage = 0;
        StopAndHideTimer();

        for (int i = 0; i < stages.Length; i++)
        {
            SetEnterWalls(i, true);
            SetCloseBehindWalls(i, false);

            if (stages[i].variants.Length > 0)
            {
                foreach (var v in stages[i].variants) v.visualObject.SetActive(false);
                stages[i].variants[0].visualObject.SetActive(true);
                stages[i].currentVariantIndex = 0;

                stages[i].upperTrigger.SetCorrect(stages[i].variants[0].isUpperTriggerCorrect);
                stages[i].lowerTrigger.SetCorrect(!stages[i].variants[0].isUpperTriggerCorrect);
            }
        }
        SetEnterWalls(0, false);
    }

    private void SetEnterWalls(int idx, bool v) { var s = GetStage(idx); if (s != null && s.enterWalls != null) s.enterWalls.SetActive(v); }
    private void SetCloseBehindWalls(int idx, bool v) { var s = GetStage(idx); if (s != null && s.closeBehindWalls != null) s.closeBehindWalls.SetActive(v); }
    private StageData GetStage(int idx) { foreach (var s in stages) if (s.stageIndex == idx) return s; return null; }
    private void Teleport(Vector3 pos) { playerRb.velocity = Vector2.zero; player.position = pos; }
}