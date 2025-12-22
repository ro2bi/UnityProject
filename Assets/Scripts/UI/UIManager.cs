using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManagerNew : MonoBehaviour
{
    public static UIManagerNew Instance { get; private set; }

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Death Screen (Checkpoint)")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private AudioClip deathSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Interaction Prompt")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    private PlayerMovementNew player;

    private void Awake()
    {
        // Настройка Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        player = FindObjectOfType<PlayerMovementNew>();

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        // 👇 НОВОЕ: Скрываем экран смерти
        if (deathScreen != null)
            deathScreen.SetActive(false);

        // Скрываем текст взаимодействия при старте
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    #region Interaction Prompt Functions
    /// <summary>
    /// Показать подсказку взаимодействия с заданным текстом.
    /// </summary>
    public static void ShowInteractionPrompt(string textToShow)
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.text = textToShow;
            Instance.interactionPromptText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Скрыть подсказку взаимодействия.
    /// </summary>
    public static void HideInteractionPrompt()
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Death & Game Over Functions
    /// <summary>
    /// 💀 ЭКРАН СМЕРТИ (есть чекпоинт) — показывает кнопку Retry
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            Time.timeScale = 0f; // Ставим игру на паузу

            if (deathSound != null)
                SoundManager.instance?.PlaySound(deathSound);
        }
    }

    

    /// <summary>
    /// ☠️ GAME OVER (нет чекпоинта) — показывает экран Game Over
    /// </summary>
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;

        if (gameOverSound != null)
            SoundManager.instance?.PlaySound(gameOverSound);
    }
    #endregion

    #region Scene Management
    private void Update()
    {
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)))
        {
            // If pause screen already active unpause and viceversa
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    /// <summary>
    /// Restart level
    /// </summary>
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Activate main menu
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Quit game/exit play mode if in Editor
    /// </summary>
    public void Quit()
    {
        Application.Quit(); // Quits the game (only works in build)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits play mode
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        // If status == true pause | if status == false unpause
        pauseScreen.SetActive(status);

        // When pause status is true change timescale to 0 (time stops)
        // when it's false change it back to 1 (time goes by normally)
        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    #endregion
}