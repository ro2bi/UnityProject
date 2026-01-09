using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

[System.Serializable]
public class MathTimingLevel
{
    public string equationText;
    public string equationText2;
    public float minValidX;
    public float maxValidX;
}

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

    [Header("Эффекты ошибки")]
    public float shakeDuration = 0.3f; // Длительность тряски
    public float shakeMagnitude = 10f;  // Интенсивность (на сколько пикселей прыгает)
    private Vector3 originalPanelPos;

    [Header("Эффекты успеха")]
    public Image sliderBarImage; // Ссылка на Image полоски (Bar)
    public float successDelay = 1f; // Задержка в 1 секунду
    private Color originalBarColor;
    private bool isWaitingNextLevel = false; // Блокировка ввода во время паузы

    private List<MathTimingLevel> levels;

    private int currentLevelIndex = 0;
    private bool isActive = false;
    private float currentXValue;
    private Action onWinCallback; // Что произойдет после победы

    public static bool IsOpen => Instance != null && Instance.minigamePanel.activeSelf;

    private void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
        if (sliderBarImage != null) originalBarColor = sliderBarImage.color;
    }

    // Запуск игры
    public void StartMinigame(List<MathTimingLevel> customLevels, Action onSuccess)
    {
        if (customLevels == null || customLevels.Count == 0)
        {
            Debug.LogError("Мини-игре не переданы уровни!");
            return;
        }

        levels = customLevels; // Устанавливаем уровни именно этого объекта
        onWinCallback = onSuccess;
        currentLevelIndex = 0;
        minigamePanel.SetActive(true);
        isActive = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        originalPanelPos = minigamePanel.transform.localPosition;

        LoadLevel();
    }

    private System.Collections.IEnumerator ShakePanel()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Вычисляем случайное смещение
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            // Применяем смещение к локальной позиции панели
            minigamePanel.transform.localPosition = new Vector3(originalPanelPos.x + x, originalPanelPos.y + y, originalPanelPos.z);

            // Используем unscaledDeltaTime, так как время на паузе!
            elapsed += Time.unscaledDeltaTime;

            yield return null; // Ждем следующий кадр
        }

        // Возвращаем панель в исходную точку
        minigamePanel.transform.localPosition = originalPanelPos;
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
        if (isWaitingNextLevel) return; // Не даем нажимать во время паузы

        var level = levels[currentLevelIndex];
        if (currentXValue >= level.minValidX && currentXValue <= level.maxValidX)
        {
            // Запускаем корутину успеха
            StartCoroutine(SuccessFlash());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShakePanel());
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

    private System.Collections.IEnumerator SuccessFlash()
    {
        isWaitingNextLevel = true; // Блокируем ввод

        // Окрашиваем в зеленый
        if (sliderBarImage != null) sliderBarImage.color = Color.green;

        // Ждем 1 секунду (реального времени)
        yield return new WaitForSecondsRealtime(successDelay);

        // Возвращаем цвет
        if (sliderBarImage != null) sliderBarImage.color = originalBarColor;

        currentLevelIndex++;
        isWaitingNextLevel = false; // Разблокируем ввод
        LoadLevel();
    }
}