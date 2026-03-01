using UnityEngine;
using UnityEngine.UI;

namespace EquationSystem
{
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
            [Header("Walls & Teleports")]
            public GameObject enterWalls;        // Стены впереди
            public GameObject closeBehindWalls;  // Стены сзади
            public Transform teleportAfterStage; // Куда летим после победы

            [Header("Triggers & Variants")]
            public ChoiceTrigger upperTrigger;
            public ChoiceTrigger lowerTrigger;
            public EquationVariant[] variants;
            public float timeLimit = 10f;
            [HideInInspector] public int currentVariantIndex = 0;
        }

        [Header("Global Settings")]
        [SerializeField] private Transform player;
        [SerializeField] private Rigidbody2D playerRb;
        [SerializeField] private Transform loseTeleportPoint;
        [SerializeField] private Transform startPositionPoint;

        [Header("UI")]
        [SerializeField] private Slider timerSlider;
        [SerializeField] private GameObject sliderContainer;

        [Header("Stages")]
        [SerializeField] private StageData[] stages;

        private int currentStage = 0;
        private float timerValue;
        private bool isTimerActive = false;

        private void Start()
        {
            if (sliderContainer != null) sliderContainer.SetActive(false);
            ResetLevel(); // Полный сброс при старте
        }

        private void Update()
        {
            if (!isTimerActive) return;

            timerValue -= Time.deltaTime;

            if (timerSlider != null)
            {
                float currentLimit = stages[currentStage].timeLimit;
                timerSlider.value = Mathf.Clamp01(timerValue / currentLimit);
            }

            if (timerValue <= 0)
            {
                SwitchVariant(currentStage);
            }
        }

        // Запуск из триггера в начале
        public void StartLevelTimer()
        {
            currentStage = 0;
            isTimerActive = true;
            if (sliderContainer != null) sliderContainer.SetActive(true);
            ResetTimerForStage(0);
        }

        // --- ЛОГИКА ВЫБОРА ---

        public void OnCorrectChoice(int stageIdx)
        {
            if (stageIdx != currentStage) return;

            StageData stage = stages[stageIdx];

            // 1. Закрываем путь назад
            if (stage.closeBehindWalls != null) stage.closeBehindWalls.SetActive(true);

            // 2. Телепортируем вперед
            if (stage.teleportAfterStage != null)
                Teleport(stage.teleportAfterStage.position);

            // 3. Переходим к следующему этапу
            currentStage++;

            if (currentStage >= stages.Length)
            {
                // ПОБЕДА (Конец уровня)
                StopLevelTimer();
            }
            else
            {
                // ОТКРЫВАЕМ СЛЕДУЮЩИЙ ЭТАП
                if (stages[currentStage].enterWalls != null)
                    stages[currentStage].enterWalls.SetActive(false);

                // СБРАСЫВАЕМ ТАЙМЕР ДЛЯ НОВОГО ЭТАПА
                ResetTimerForStage(currentStage);
            }
        }

        public void OnWrongChoice()
        {
            // При ошибке: меняем пример, стопаем таймер и на старт
            SwitchVariant(currentStage);
            StopLevelTimer();
            Teleport(loseTeleportPoint.position);
            ResetLevel(); // Сбрасываем все стены в исходное состояние
            Teleport(startPositionPoint.position);
        }

        // --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ---

        private void SwitchVariant(int stageIdx)
        {
            StageData stage = stages[stageIdx];
            stage.variants[stage.currentVariantIndex].visualObject.SetActive(false);

            stage.currentVariantIndex = (stage.currentVariantIndex + 1) % stage.variants.Length;

            EquationVariant newVar = stage.variants[stage.currentVariantIndex];
            newVar.visualObject.SetActive(true);
            stage.upperTrigger.SetCorrect(newVar.isUpperTriggerCorrect);
            stage.lowerTrigger.SetCorrect(!newVar.isUpperTriggerCorrect);

            ResetTimerForStage(stageIdx);
        }

        private void ResetTimerForStage(int stageIdx)
        {
            timerValue = stages[stageIdx].timeLimit;
            if (timerSlider != null) timerSlider.value = 1f;
        }

        private void StopLevelTimer()
        {
            isTimerActive = false;
            if (sliderContainer != null) sliderContainer.SetActive(false);
        }

        private void ResetLevel()
        {
            currentStage = 0;
            StopLevelTimer();

            for (int i = 0; i < stages.Length; i++)
            {
                // Стены входа: первого открываем, остальные закрываем
                if (stages[i].enterWalls != null) stages[i].enterWalls.SetActive(i != 0);
                // Стены сзади: все убираем
                if (stages[i].closeBehindWalls != null) stages[i].closeBehindWalls.SetActive(false);

                // Сброс вариантов примеров
                if (stages[i].variants.Length > 0)
                {
                    foreach (var v in stages[i].variants) v.visualObject.SetActive(false);
                    stages[i].variants[0].visualObject.SetActive(true);
                    stages[i].currentVariantIndex = 0;

                    stages[i].upperTrigger.SetCorrect(stages[i].variants[0].isUpperTriggerCorrect);
                    stages[i].lowerTrigger.SetCorrect(!stages[i].variants[0].isUpperTriggerCorrect);
                }
            }
        }

        private void Teleport(Vector3 pos)
        {
            playerRb.velocity = Vector2.zero;
            player.position = pos;
        }
    }
}