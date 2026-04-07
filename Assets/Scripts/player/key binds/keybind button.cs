using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using System;

public class RebindButton : MonoBehaviour
{
    [Header("��������� ��������")]
    [Tooltip("��� ��������, ������� ������ ��������� � ���������� � KeybindManager (��������, 'Jump').")]
    public string actionToRebind;

    [Header("������ �� UI")]
    [Tooltip("��������� ���������, ������� ���������� ������� ������� (��������, 'Space').")]
    [SerializeField] private TMP_Text keyText;

    [Tooltip("��������� ��������� ��� ��������� 'PRESS NEW KEY...'")]
    [SerializeField] private TMP_Text promptText;

    [Tooltip("������, ������� �������� ������������.")]
    private Button button;

    private KeyCode currentKey;
    private bool isRebinding = false;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError($"RebindButton requires a Button component on GameObject {gameObject.name}");
            return;
        }

        button.onClick.AddListener(StartRebinding);
    }

    void Start()
    {
        KeybindManager.InitializeKeys();

        UpdateKeyText();

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    public void StartRebinding()
    {
        if (string.IsNullOrEmpty(actionToRebind))
        {
            Debug.LogError("ActionToRebind name is empty for " + gameObject.name);
            return;
        }

        if (!isRebinding)
        {
            isRebinding = true;

            if (keyText != null)
            {
                keyText.gameObject.SetActive(false);
            }
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
            }

            StartCoroutine(WaitForInput());
        }
    }

    public void UpdateKeyText()
    {
        if (keyText != null && !string.IsNullOrEmpty(actionToRebind))
        {
            currentKey = KeybindManager.GetKey(actionToRebind);

            keyText.text = currentKey.ToString().Replace("KeyCode.", "");
            keyText.gameObject.SetActive(true);
        }
    }

    private IEnumerator WaitForInput()
    {
        yield return null;

        KeyCode newKey = KeyCode.None;

        while (newKey == KeyCode.None)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (IsInvalidKey(key))
                    {
                        continue;
                    }

                    newKey = key;
                    break;
                }
            }

            if (newKey == KeyCode.None)
            {
                yield return null;
            }
        }

        RebindComplete(newKey);
    }

    private bool IsInvalidKey(KeyCode key)
    {
        if ((int)key >= (int)KeyCode.Mouse0 && (int)key <= (int)KeyCode.Mouse6)
        {
            return true;
        }

        if (key == KeyCode.None || key == KeyCode.Menu)
        {
            return true;
        }

        return false;
    }

    private void RebindComplete(KeyCode newKey)
    {
        KeybindManager.SetKey(actionToRebind, newKey);

        currentKey = newKey;
        UpdateKeyText();

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        isRebinding = false;

        Debug.Log($"Rebind for {actionToRebind} finished. New key: {newKey}");
    }
}