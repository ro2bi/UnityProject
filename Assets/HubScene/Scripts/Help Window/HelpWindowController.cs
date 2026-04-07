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

    [Header("��������� ������")]
    public Image buttonIcon;     
    public Sprite iconGame;       
    public Sprite iconMenuOpen;     

    [Header("�������� �������")]
    public GameObject startupHintObject; 
    public float hintDuration = 5f;

    [Header("�������������� ����������")]
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
        UpdateIconButton(false);
        Time.timeScale = 1f;
        MineGridManager2D.IsUIOpen = false;
    }

    
    private void UpdateHelpContent()
    {
        if (textDisplay == null) return;
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(helpTextContent))
        {
            sb.AppendLine("<size=120%><color=#FFCC00></color></size>");
            sb.AppendLine(helpTextContent);
            sb.AppendLine();
        }
        sb.AppendLine("<size=120%><color=#FFCC00>���������:</color></size>");
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
            case KeybindManager.MOVE_FORWARD: return "������";
            case KeybindManager.MOVE_BACKWARD: return "�����";
            case KeybindManager.MOVE_LEFT: return "����";
            case KeybindManager.MOVE_RIGHT: return "������";
            case KeybindManager.INTERACT: return "�������";
            case KeybindManager.INVENTORY: return "��������";
            case KeybindManager.DROP: return "�������� �������";
            case KeybindManager.JUMP: return "�������";
            case KeybindManager.HELPWINDOW: return "³������ ��������";
            case KeybindManager.TOMENU: return "����";
            default: return actionKey;
        }
    }
    public TextMeshProUGUI startupHintTextDisplay; 

    private System.Collections.IEnumerator HideHintAfterTime()
    {
        KeyCode helpKey = KeybindManager.GetKey(KeybindManager.HELPWINDOW);

        if (startupHintTextDisplay != null)
        {
            startupHintTextDisplay.text = $"{helpKey} ��� ���������� ���������";
        }

        startupHintObject.SetActive(true);
        yield return new WaitForSeconds(hintDuration);
        startupHintObject.SetActive(false);
    }
}