using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class HelpWindowController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject helpPanel;
    public TextMeshProUGUI textDisplay;

    [Header("Настройки Иконок")]
    public Image buttonIcon;     
    public Sprite iconGame;       
    public Sprite iconMenuOpen;     

    [Header("Стартова підказка")]
    public GameObject startupHintObject; 
    public float hintDuration = 5f;

    [Header("Дополнительная информация")]
    [TextArea(5, 15)]
    public string helpTextContent;

    private void Start()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
        UpdateHelpContent();
        UpdateIconButton(false);

        if (startupHintObject != null)
        {
            StartCoroutine(HideHintAfterTime());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.HELPWINDOW)))
        {
            ToggleHelp();
        }

        if (helpPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseHelp();
        }
    }

    public void ToggleHelp()
    {
        if (helpPanel == null) return;

        bool isOpening = !helpPanel.activeSelf;

        if (isOpening)
        {
            UpdateHelpContent();
        }

        helpPanel.SetActive(isOpening);

        UpdateIconButton(isOpening);

        Time.timeScale = isOpening ? 0f : 1f;
        MineGridManager2D.IsUIOpen = isOpening;

        if (startupHintObject != null) startupHintObject.SetActive(false);
    }

    private void UpdateIconButton(bool isOpen)
    {
        if (buttonIcon != null)
        {
            buttonIcon.sprite = isOpen ? iconMenuOpen : iconGame;
        }
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
        UpdateIconButton(false); // Возвращаем иконку игры
        Time.timeScale = 1f;
        MineGridManager2D.IsUIOpen = false;
    }

    
    private void UpdateHelpContent()
    {
        if (textDisplay == null) return;
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(helpTextContent))
        {
            sb.AppendLine("<size=120%><color=#FFCC00>INFO:</color></size>");
            sb.AppendLine(helpTextContent);
            sb.AppendLine();
        }
        sb.AppendLine("<size=120%><color=#FFCC00>Управління:</color></size>");
        var allBinds = KeybindManager.GetAllKeybinds();
        foreach (var bind in allBinds)
        {
            sb.AppendLine($"{GetFriendlyName(bind.Key)} --- {bind.Value}");
        }
        textDisplay.text = sb.ToString();
    }

    private string GetFriendlyName(string actionKey)
    {
        switch (actionKey)
        {
            case KeybindManager.MOVE_FORWARD: return "Вперед";
            case KeybindManager.MOVE_BACKWARD: return "Назад";
            case KeybindManager.MOVE_LEFT: return "Вліво";
            case KeybindManager.MOVE_RIGHT: return "Вправо";
            case KeybindManager.INTERACT: return "Взаємодія";
            case KeybindManager.INVENTORY: return "Інвентар";
            case KeybindManager.DROP: return "Викинуть предмет";
            case KeybindManager.JUMP: return "Стрибок";
            case KeybindManager.HELPWINDOW: return "Відкрити допомогу";
            case KeybindManager.TOMENU: return "Меню";
            default: return actionKey;
        }
    }
    private System.Collections.IEnumerator HideHintAfterTime()
    {
        startupHintObject.SetActive(true);
        yield return new WaitForSeconds(hintDuration);
        startupHintObject.SetActive(false);
    }
}