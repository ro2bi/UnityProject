using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Interaction Prompt")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;

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

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        // НОВОЕ: Скрываем текст взаимодействия при старте
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    #region Interaction Prompt Functions

    //Показать подсказку взаимодействия с заданным текстом.</summary>
    public static void ShowInteractionPrompt(string textToShow)
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.text = textToShow;
            Instance.interactionPromptText.gameObject.SetActive(true);
        }
    }

    //Скрыть подсказку взаимодействия.
    public static void HideInteractionPrompt()
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Game Over Functions
    //Game over function
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)))
        {
            //If pause screen already active unpause and viceversa
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    //Restart level
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Activate game over screen
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
        Time.timeScale = 1;
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        Application.Quit(); //Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Exits play mode
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        //If status == true pause | if status == false unpause
        pauseScreen.SetActive(status);

        //When pause status is true change timescale to 0 (time stops)
        //when it's false change it back to 1 (time goes by normally)
        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.1f);
    }
    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.1f);
    }
    #endregion
}
