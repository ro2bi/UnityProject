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

    [Header("UI ��������")]
    public GameObject minigamePanel;
    public TextMeshProUGUI equationText;
    public TextMeshProUGUI equationText2;
    public RectTransform sliderHandle;
    public RectTransform sliderBar;
    public TextMeshProUGUI progressText;

    [Header("���������")]
    public float minValue = 0f;
    public float maxValue = 10f;
    public float sliderSpeed = 2f;

    [Header("������� ������")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 10f;
    private Vector3 originalPanelPos;

    [Header("������� ������")]
    public Image sliderBarImage;
    public float successDelay = 1f;
    private Color originalBarColor;
    private bool isWaitingNextLevel = false;

    private List<MathTimingLevel> levels;

    private int currentLevelIndex = 0;
    private bool isActive = false;
    private float currentXValue;
    private Action onWinCallback;

    public static bool IsOpen => Instance != null && Instance.minigamePanel.activeSelf;

    private void Awake()
    {
        Instance = this;
        minigamePanel.SetActive(false);
        if (sliderBarImage != null) originalBarColor = sliderBarImage.color;
    }

    public void StartMinigame(List<MathTimingLevel> customLevels, Action onSuccess)
    {
        if (customLevels == null || customLevels.Count == 0)
        {
            Debug.LogError("����-���� �� �������� ������!");
            return;
        }

        levels = customLevels;
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
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            minigamePanel.transform.localPosition = new Vector3(originalPanelPos.x + x, originalPanelPos.y + y, originalPanelPos.z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        minigamePanel.transform.localPosition = originalPanelPos;
    }

    private void Update()
    {
        if (!isActive) return;

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
        if (isWaitingNextLevel) return;

        var level = levels[currentLevelIndex];
        if (currentXValue >= level.minValidX && currentXValue <= level.maxValidX)
        {
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

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onWinCallback?.Invoke();
    }

    private System.Collections.IEnumerator SuccessFlash()
    {
        isWaitingNextLevel = true;

        if (sliderBarImage != null) sliderBarImage.color = Color.green;

        yield return new WaitForSecondsRealtime(successDelay);

        if (sliderBarImage != null) sliderBarImage.color = originalBarColor;

        currentLevelIndex++;
        isWaitingNextLevel = false;
        LoadLevel();
    }
}