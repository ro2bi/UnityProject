using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject keybindsScreen;
    [SerializeField] private GameObject mainGameCanvas;

    [Header("��������� ����������")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private bool isFullScreen = true;

    [Header("������������� �����")]
    public Image checkmarkImage;
    public Sprite checkSprite;
    public Sprite crossSprite;

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
       
    }

    void Start()
    {
        if (resolutionDropdown != null)
        {
            InitializeResolutionDropdown();
        }

        if (keybindsScreen != null)
        {
            keybindsScreen.SetActive(false);
        }

        KeybindManager.InitializeKeys();
    }

    public void StartGame()
    {
        Debug.Log("Start pressed!");
        SceneManager.LoadScene("HubScene"); 
    }

    public void ResetBinds()
    {
        KeybindManager.ResetToDefaults();
    }

    public void PauseGame()
    {
        Debug.Log("Pause/Resume pressed!");
        PauseGame(!pauseScreen.activeInHierarchy);
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed!");
        Application.Quit();
    }

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);

        if (mainGameCanvas != null)
        {
            mainGameCanvas.SetActive(!status);
        }
        Debug.Log("pause pressed!");
    }
    #endregion

    public void PauseGame2(bool status)
    {
        keybindsScreen.SetActive(false);
    }

    #region KeybindsScreen
    public void ToggleKeybindsScreen(bool showKeybinds)
    {
        if (pauseScreen == null || keybindsScreen == null)
        {
            Debug.LogError("PauseScreen or KeybindsScreen is not assigned in the Inspector.");
            return;
        }

        pauseScreen.SetActive(!showKeybinds);
        keybindsScreen.SetActive(showKeybinds);

        if (!showKeybinds)
        {
        }
    }
    #endregion

    private void InitializeResolutionDropdown()
    {
        resolutions = Screen.resolutions
        .GroupBy(res => new { res.width, res.height })
        .Select(g => g.First())
        .ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Debug.Log("Available unique resolutions found:");
        foreach (var res in resolutions)
        {
            Debug.Log($"- {res.width}x{res.height} @ {res.refreshRateRatio.value:0.00} Hz");
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
        Debug.Log($"Resolution set to: {resolution.width}x{resolution.height}");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        isFullScreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscreen set to: {isFullscreen}");
    }

    public void SetVSync(bool isVSyncEnabled)
    {
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;

        if (isVSyncEnabled)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        if (checkmarkImage != null)
        {
            checkmarkImage.sprite = isVSyncEnabled ? checkSprite : crossSprite;
        }
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
}