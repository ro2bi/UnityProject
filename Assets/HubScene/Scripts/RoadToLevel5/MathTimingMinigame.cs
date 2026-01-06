using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class MathTimingMinigame : MonoBehaviour
{
    public static MathTimingMinigame Instance { get; private set; }

    [Header("UI элементы")]
    public GameObject minigamePanel;
    public TextMeshProUGUI equationText;
    public TextMeshProUGUI equationText2;
    public RectTransform sliderHandle;
    public RectTransform sliderBar;
    public TextMeshProUGUI progressText;

    [Header("Настройки")]
    public float minValue = 0f;
    public float maxValue = 10f;
    public float sliderSpeed = 2f;

    [System.Serializable]
    public class MathTimingLevel
    {
        public string equationText;
        public string equationText2;
        public float minValidX;
        public float maxValidX;
    }

    public List<MathTimingLevel> levels = new List<MathTimingLevel>();

    private int currentLevelIndex = 0;
    private bool isActive = false;
    private float currentXValue;
    private Action onWinCallback; // Что произойдет после победы

    public static bool IsOpen => Instance != null && Instance.minigamePanel.activeSelf;

    private void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
    }

    // Запуск игры
    public void StartMinigame(Action onSuccess)
    {
        onWinCallback = onSuccess;
        currentLevelIndex = 0;
        minigamePanel.SetActive(true);
        isActive = true;

        // ЗАМОРАЖИВАЕМ ВРЕМЯ
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadLevel();
    }

    private void Update()
    {
        if (!isActive) return;

        // Используем unscaledTime, так как обычное время стоит
        float t = Mathf.PingPong(Time.unscaledTime * sliderSpeed, 1f);
        currentXValue = Mathf.Lerp(minValue, maxValue, t);

        float barWidth = sliderBar.rect.width;
        sliderHandle.anchoredPosition = new Vector2(Mathf.Lerp(-barWidth / 2, barWidth / 2, t), 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckAnswer();
        }
    }

    private void LoadLevel()
    {
        if (currentLevelIndex < levels.Count)
        {
            var level = levels[currentLevelIndex];
            equationText.text = level.equationText;
            equationText2.text = level.equationText2;
            progressText.text = $"{currentLevelIndex + 1} / {levels.Count}";
        }
        else
        {
            WinMinigame();
        }
    }

    private void CheckAnswer()
    {
        var level = levels[currentLevelIndex];
        if (currentXValue >= level.minValidX && currentXValue <= level.maxValidX)
        {
            currentLevelIndex++;
            LoadLevel();
        }
        else
        {
            Debug.Log("Промах! Уровень заново.");
            // Можно добавить визуальный эффект ошибки
        }
    }

    private void WinMinigame()
    {
        isActive = false;
        minigamePanel.SetActive(false);

        // РАЗМОРАЖИВАЕМ ВРЕМЯ
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Выполняем действие (удаление объекта и спавн предмета)
        onWinCallback?.Invoke();
    }
}