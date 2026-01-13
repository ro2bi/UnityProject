using UnityEngine;
using TMPro;

public class HelpWindowController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject helpPanel;
    [TextArea(5, 10)]
    public string helpTextContent;
    public TextMeshProUGUI textDisplay;

    // Ссылка на имя клавиши в KeybindManager
    private const string HELP_KEY = "HelpWindow";

    private void Start()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
        if (textDisplay != null) textDisplay.text = helpTextContent;
    }

    private void Update()
    {
        // Проверяем нажатие клавиши из твоего KeybindManager
        if (Input.GetKeyDown(KeybindManager.GetKey(HELP_KEY)))
        {
            ToggleHelp();
        }

        // Если окно открыто, позволяем закрыть его на Escape (опционально)
        if (helpPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHelp();
        }
    }

    public void ToggleHelp()
    {
        if (helpPanel == null) return;

        bool isOpening = !helpPanel.activeSelf;
        helpPanel.SetActive(isOpening);

        // Ставим паузу
        Time.timeScale = isOpening ? 0f : 1f;

        // Блокируем игрока
        MineGridManager2D.IsUIOpen = isOpening;
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
        Time.timeScale = 1f;
        MineGridManager2D.IsUIOpen = false;
    }
}