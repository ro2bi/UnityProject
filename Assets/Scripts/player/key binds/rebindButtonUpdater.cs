using UnityEngine;
using TMPro;

public class RebindButtonUpdater : MonoBehaviour
{
    // Строка действия, которое привязано к этой кнопке (например, "Jump")
    [SerializeField] private string actionName = "";

    // Ссылка на компонент TextMeshPro, который показывает текущую клавишу
    [SerializeField] private TMP_Text keyTextComponent;

    private void OnEnable()
    {
        // Подписываемся на событие при активации
        KeybindManager.OnKeybindsChanged += UpdateButtonText;

        // Обновляем текст при первой активации (при открытии меню)
        UpdateButtonText();
    }

    private void OnDisable()
    {
        // Отписываемся от события при деактивации, чтобы избежать утечек памяти
        KeybindManager.OnKeybindsChanged -= UpdateButtonText;
    }

    public void UpdateButtonText()
    {
        // Получаем KeyCode для нашего действия
        KeyCode currentKey = KeybindManager.GetKey(actionName);

        if (keyTextComponent != null)
        {
            // Устанавливаем текст (например, "JUMP: SPACE")
            // Предполагаем, что ваш текст состоит из двух частей: ИмяДействия: Клавиша
            // Если у вас текст только для клавиши, используйте:
            keyTextComponent.text = currentKey.ToString();
        }
    }
}
