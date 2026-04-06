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

        if (deathScreen != null)
            deathScreen.SetActive(false);

        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    
    
    public static void ShowInteractionPrompt(string textToShow)
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.text = textToShow;
            Instance.interactionPromptText.gameObject.SetActive(true);
        }
    }

    
    public static void HideInteractionPrompt()
    {
        if (Instance != null && Instance.interactionPromptText != null)
        {
            Instance.interactionPromptText.gameObject.SetActive(false);
        }
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
            Time.timeScale = 0f; 

            if (deathSound != null)
                SoundManager.instance?.PlaySound(deathSound);
        }
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;

        if (gameOverSound != null)
            SoundManager.instance?.PlaySound(gameOverSound);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)))
        {
            if (UIManagerShop.IsWindowOpen || UIManagerShop.EscPressedThisFrame)
            {
                return;
            }

            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void Quit()
    {
        Application.Quit(); 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Exits play mode
#endif
    }

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
}