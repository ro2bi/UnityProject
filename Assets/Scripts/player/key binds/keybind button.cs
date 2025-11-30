using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using System; // Важно: добавляем для работы с Enum (KeyCode)

public class RebindButton : MonoBehaviour
{
    [Header("Настройки Привязки")]
    [Tooltip("Имя действия, которое должно совпадать с константой в KeybindManager (например, 'Jump').")]
    public string actionToRebind;

    [Header("Ссылки на UI")]
    [Tooltip("Текстовый компонент, который показывает текущую клавишу (например, 'Space').")]
    [SerializeField] private TMP_Text keyText;

    [Tooltip("Текстовый компонент для сообщения 'PRESS NEW KEY...'")]
    [SerializeField] private TMP_Text promptText;

    [Tooltip("Кнопка, которую нажимает пользователь.")]
    private Button button;

    private KeyCode currentKey;
    private bool isRebinding = false; // Флаг, чтобы избежать двойного запуска

    // ----------------------------------------------------------------------
    // UNITY МЕТОДЫ
    // ----------------------------------------------------------------------

    void Awake()
    {
        // Получаем компонент Button, к которому прикреплен этот скрипт
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError($"RebindButton requires a Button component on GameObject {gameObject.name}");
            return;
        }

        // Добавляем слушателя, который будет запускать переназначение при клике
        button.onClick.AddListener(StartRebinding);
    }

    void Start()
    {
        // 1. Инициализируем KeybindManager (если еще не инициализирован)
        // (Это нужно для корректной работы, если RebindButton появляется первым)
        KeybindManager.InitializeKeys();

        // 2. Обновляем текст на кнопке при старте
        UpdateKeyText();

        // Убеждаемся, что текст-подсказка скрыт по умолчанию
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    // ----------------------------------------------------------------------
    // ПУБЛИЧНЫЕ МЕТОДЫ
    // ----------------------------------------------------------------------

    // Вызывается из Button.onClick, когда пользователь хочет переназначить клавишу
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
            // 1. Скрываем текущий текст клавиши и показываем подсказку
            if (keyText != null)
            {
                keyText.gameObject.SetActive(false);
            }
            if (promptText != null)
            {
                // ПОЯВЛЯЕТСЯ "PRESS NEW KEY..."
                promptText.gameObject.SetActive(true);
            }

            // 2. Запускаем ожидание ввода
            StartCoroutine(WaitForInput());
        }
    }

    // Обновляет текст на кнопке, используя текущую привязку из KeybindManager
    // Эту функцию вызывает MenuManager, когда возвращается из KeybindsScreen.
    public void UpdateKeyText()
    {
        if (keyText != null && !string.IsNullOrEmpty(actionToRebind))
        {
            // Получаем текущую клавишу
            currentKey = KeybindManager.GetKey(actionToRebind);

            // Отображаем ее название (удаляя "KeyCode." префикс)
            keyText.text = currentKey.ToString().Replace("KeyCode.", "");
            keyText.gameObject.SetActive(true); // Убеждаемся, что текст клавиши виден
        }
    }

    // ----------------------------------------------------------------------
    // ЛОГИКА ОЖИДАНИЯ ВВОДА
    // ----------------------------------------------------------------------

    private IEnumerator WaitForInput()
    {
        // Ждем один кадр, чтобы сбросить нажатие кнопки мыши, которое запустило корутину
        yield return null;

        KeyCode newKey = KeyCode.None;

        // Бесконечный цикл ожидания нажатия
        while (newKey == KeyCode.None)
        {
            // Перебираем все возможные KeyCode (кроме служебных, вроде None)
            // Мы используем System.Enum.GetValues, для чего нужен 'using System;'
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                // Проверяем, была ли нажата клавиша в этом кадре
                if (Input.GetKeyDown(key))
                {
                    // Игнорируем клавиши, которые не подходят для управления (например, мышь)
                    if (IsInvalidKey(key))
                    {
                        continue;
                    }

                    newKey = key;
                    break; // Нашли нажатую клавишу, выходим из цикла foreach
                }
            }

            if (newKey == KeyCode.None)
            {
                yield return null; // Ждем следующего кадра
            }
        }

        // Ввод получен, завершаем процесс
        RebindComplete(newKey);
    }

    // Проверяет, является ли клавиша непригодной для переназначения (например, мышь, джойстик)
    private bool IsInvalidKey(KeyCode key)
    {
        // Игнорируем все кнопки мыши (Mouse0, Mouse1, Mouse2 и т.д.)
        if ((int)key >= (int)KeyCode.Mouse0 && (int)key <= (int)KeyCode.Mouse6)
        {
            return true;
        }

        // Также игнорируем служебные клавиши, которые могут сработать некорректно
        if (key == KeyCode.None || key == KeyCode.Menu)
        {
            return true;
        }

        return false;
    }

    // ----------------------------------------------------------------------
    // ЗАВЕРШЕНИЕ
    // ----------------------------------------------------------------------

    private void RebindComplete(KeyCode newKey)
    {
        // 1. Записываем новую клавишу в KeybindManager
        KeybindManager.SetKey(actionToRebind, newKey);

        // 2. Обновляем текст на кнопке
        currentKey = newKey;
        UpdateKeyText();

        // 3. Скрываем подсказку и восстанавливаем состояние
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false); // Скрываем "PRESS NEW KEY..."
        }
        isRebinding = false;

        Debug.Log($"Rebind for {actionToRebind} finished. New key: {newKey}");
    }
}
