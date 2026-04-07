using UnityEngine;
using TMPro;

public class RebindButtonUpdater : MonoBehaviour
{
    [SerializeField] private string actionName = "";

    [SerializeField] private TMP_Text keyTextComponent;

    private void OnEnable()
    {
        KeybindManager.OnKeybindsChanged += UpdateButtonText;

        UpdateButtonText();
    }

    private void OnDisable()
    {
        KeybindManager.OnKeybindsChanged -= UpdateButtonText;
    }

    public void UpdateButtonText()
    {
        KeyCode currentKey = KeybindManager.GetKey(actionName);

        if (keyTextComponent != null)
        {
            keyTextComponent.text = currentKey.ToString();
        }
    }
}