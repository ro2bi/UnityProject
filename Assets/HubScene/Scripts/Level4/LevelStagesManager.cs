using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Нужно для работы корутин

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
            public GameObject enterWalls;
            public GameObject closeBehindWalls;
            public Transform teleportAfterStage;

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

        // --- НОВЫЙ БЛОК: НАСТРОЙКИ ФИНАЛА ---
        [Header("End Level Visuals")]
        [Tooltip("Объекты, которые выключатся навсегда после победы")]
        [SerializeField] private GameObject[] objectsToDeactivate;

        [Tooltip("Объекты, которые включатся только на время")]
        [SerializeField] private GameObject[] objectsToShowTemporarily;

        [SerializeField] private float temporaryShowDuration = 3f;
        // ------------------------------------

        private int currentStage = 0;
        private float timerValue;
        private bool isTimerActive = false;

        private void Start()
        {
            if (sliderContainer != null) sliderContainer.SetActive(false);
            ResetLevel();
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

        public void StartLevelTimer()
        {
            currentStage = 0;
            isTimerActive = true;
            if (sliderContainer != null) sliderContainer.SetActive(true);
            ResetTimerForStage(0);
        }

        public void OnCorrectChoice(int stageIdx)
        {
            if (stageIdx != currentStage) return;

            StageData stage = stages[stageIdx];

            if (stage.closeBehindWalls != null) stage.closeBehindWalls.SetActive(true);

            if (stage.teleportAfterStage != null)
                Teleport(stage.teleportAfterStage.position);

            currentStage++;

            if (currentStage >= stages.Length)
            {
                // ПОБЕДА
                StopLevelTimer();
                StartCoroutine(EndLevelSequence()); // ЗАПУСК ФИНАЛЬНОЙ ЛОГИКИ
            }
            else
            {
                if (stages[currentStage].enterWalls != null)
                    stages[currentStage].enterWalls.SetActive(false);

                ResetTimerForStage(currentStage);
            }
        }

        // Корутина для финала
        private IEnumerator EndLevelSequence()
        {
            // 1. Выключаем объекты, которые не нужны в конце
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null) obj.SetActive(false);
            }

            // 2. Включаем временные объекты (например, надпись "Победа" или эффекты)
            foreach (GameObject obj in objectsToShowTemporarily)
            {
                if (obj != null) obj.SetActive(true);
            }

            // 3. Ждем указанное время
            yield return new WaitForSeconds(temporaryShowDuration);

            // 4. Выключаем временные объекты обратно
            foreach (GameObject obj in objectsToShowTemporarily)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        public void OnWrongChoice()
        {
            SwitchVariant(currentStage);
            StopLevelTimer();
            Teleport(loseTeleportPoint.position);
            ResetLevel();
            Teleport(startPositionPoint.position);
        }

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
            StopAllCoroutines(); // Останавливаем финал, если игрок проиграл в процессе

            currentStage = 0;
            StopLevelTimer();

            // Сброс финала (возвращаем объекты в исходное состояние при рестарте)
            foreach (GameObject obj in objectsToDeactivate) if (obj) obj.SetActive(true);
            foreach (GameObject obj in objectsToShowTemporarily) if (obj) obj.SetActive(false);

            for (int i = 0; i < stages.Length; i++)
            {
                if (stages[i].enterWalls != null) stages[i].enterWalls.SetActive(i != 0);
                if (stages[i].closeBehindWalls != null) stages[i].closeBehindWalls.SetActive(false);

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