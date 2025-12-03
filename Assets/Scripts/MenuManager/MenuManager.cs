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
    // --- НОВОЕ ПОЛЕ: Ссылка на меню переназначения клавиш ---
    [SerializeField] private GameObject keybindsScreen;

    // --- НОВОЕ ПОЛЕ: Ссылка на главный игровой интерфейс (HUD) ---
    [SerializeField] private GameObject mainGameCanvas;

    // --- Поля для Разрешения Экрана ---
    [Header("Настройки Разрешения")]
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Компонент Dropdown для выбора разрешения
    private Resolution[] resolutions; // Массив доступных разрешений
    private bool isFullScreen = true;

    [Header("Переключатель Всунк")]
    public Image checkmarkImage;
    public Sprite checkSprite;
    public Sprite crossSprite;

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Инициализация при старте (например, для главного меню)
        if (resolutionDropdown != null)
        {
            InitializeResolutionDropdown();
        }

        // Убедимся, что KeybindsScreen изначально выключен, если он назначен
        if (keybindsScreen != null)
        {
            keybindsScreen.SetActive(false);
        }

        KeybindManager.InitializeKeys();
    }

    // --- Логика Запуска и Паузы (Не изменена) ---
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
        Debug.Log("Quit pressed!"); // only works in build
        Application.Quit();
    }

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);

        // --- НОВАЯ ЛОГИКА: Выключение основного игрового интерфейса ---
        if (mainGameCanvas != null)
        {
            // Если status = true (ПАУЗА), то mainGameCanvas.SetActive(false) -> выключаем.
            // Если status = false (ПРОДОЛЖИТЬ), то mainGameCanvas.SetActive(true) -> включаем.
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
        // --- НОВАЯ ФУНКЦИЯ ДЛЯ ПЕРЕКЛЮЧЕНИЯ ЭКРАНОВ ---
    public void ToggleKeybindsScreen(bool showKeybinds)
    {
        if (pauseScreen == null || keybindsScreen == null)
        {
            Debug.LogError("PauseScreen or KeybindsScreen is not assigned in the Inspector.");
            return;
        }

        // Если showKeybinds = true:
        // 1. Выключаем основной экран паузы
        pauseScreen.SetActive(!showKeybinds);

        // 2. Включаем экран переназначения клавиш
        keybindsScreen.SetActive(showKeybinds);

        // Если вы открываете меню Keybinds, то вы в меню. 
        // Если вы закрываете его (возвращаясь к pauseScreen), то нужно обновить UI.
        if (!showKeybinds)
        {
            // Убедитесь, что KeybindManager обновлен
            // Возможно, здесь потребуется вызвать обновление текста для каждой кнопки RebindButton, 
            // если вы не используете статический метод для этого.
        }
    }
    #endregion

    // --- Логика для Разрешения Экрана (Не изменена) ---

    // 1. Сбор и отображение доступных разрешений
    private void InitializeResolutionDropdown()
    {
        // Получаем все разрешения, поддерживаемые устройством
        // Используем LINQ для фильтрации уникальных разрешений по ширине и высоте
        resolutions = Screen.resolutions
      .GroupBy(res => new { res.width, res.height })
      .Select(g => g.First())
      .ToArray();

        resolutionDropdown.ClearOptions(); // Очищаем старые записи

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Ищем текущее разрешение, чтобы оно было выбрано по умолчанию
            if (resolutions[i].width == Screen.currentResolution.width &&
        resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options); // Добавляем все варианты
        resolutionDropdown.value = currentResolutionIndex; // Выбираем текущее
        resolutionDropdown.RefreshShownValue(); // Обновляем отображаемое значение

        // Дополнительная проверка: вывод всех найденных разрешений в консоль
        Debug.Log("Available unique resolutions found:");
        foreach (var res in resolutions)
        {
            Debug.Log($"- {res.width}x{res.height} @ {res.refreshRateRatio.value:0.00} Hz");
        }
    }

    // 2. Установка нового разрешения (вызывается из Dropdown)
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        // Устанавливаем новое разрешение, сохраняя текущий полноэкранный режим
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
        Debug.Log($"Resolution set to: {resolution.width}x{resolution.height}");
    }

    // 3. Переключение полноэкранного режима (вызывается из Toggle)
    public void SetFullscreen(bool isFullscreen)
    {
        isFullScreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        Debug.Log($"Fullscreen set to: {isFullscreen}");
    }


    public void SetVSync(bool isVSyncEnabled)
    {
        // Шаг 1: Устанавливаем V-Sync
        QualitySettings.vSyncCount = isVSyncEnabled ? 1 : 0;

        // Шаг 2: Управляем ограничением FPS
        if (isVSyncEnabled)
        {
            // V-Sync включен, отключаем ограничение FPS
            Application.targetFrameRate = -1;
        }
        else
        {
            // V-Sync выключен, устанавливаем разумное ограничение FPS (например, 60)
            // Если хотите полностью убрать ограничение, используйте -1, но 60 лучше для экономии ресурсов.
            Application.targetFrameRate = 60;
        }

        // Обновление спрайта
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
