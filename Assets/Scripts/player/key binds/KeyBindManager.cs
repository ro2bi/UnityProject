using UnityEngine;
using System.Collections.Generic;
using System;

public static class KeybindManager
{
    // Имя, используемое для сохранения данных в PlayerPrefs
    private const string KeybindsSaveKey = "GameKeybinds";

    // Словарь, хранящий текущие привязки: ActionName -> KeyCode
    private static Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    public static event Action OnKeybindsChanged;

    // КОНСТАНТЫ ДЕЙСТВИЙ: используйте эти строки в RebindButton.ActionToRebind
    public const string JUMP = "Jump";
    public const string MOVE_FORWARD = "MoveForward"; 
    public const string MOVE_BACKWARD = "MoveBackward";
    public const string MOVE_RIGHT = "MoveRight";
    public const string MOVE_LEFT = "MoveLeft";
    public const string INTERACT = "Interact"; 
    public const string TOMENU = "ToMenu";
    // Добавьте сюда любые другие действия

    // ----------------------------------------------------------------------
    // 1. Инициализация и загрузка (Вызывается один раз при старте игры/меню)
    // ----------------------------------------------------------------------

    private static readonly Dictionary<string, KeyCode> DefaultKeybinds = new Dictionary<string, KeyCode>
{
    { JUMP, KeyCode.Space },
    { MOVE_FORWARD, KeyCode.W },
    { MOVE_BACKWARD, KeyCode.S },
    { MOVE_RIGHT, KeyCode.D },
    { MOVE_LEFT, KeyCode.A },
    { INTERACT, KeyCode.E },
    { TOMENU, KeyCode.Escape }
};

    public static void InitializeKeys()
    {
        if (PlayerPrefs.HasKey(KeybindsSaveKey))
        {
            LoadKeybinds();
        }
        else
        {
            // Используем значения по умолчанию
            keybinds = new Dictionary<string, KeyCode>(DefaultKeybinds);
            SaveKeybinds(); // Сохраняем значения по умолчанию
        }
    }

    // ----------------------------------------------------------------------
    // 2. Публичные методы для доступа и изменения
    // ----------------------------------------------------------------------

    // Получить KeyCode для определенного действия
    public static KeyCode GetKey(string actionName)
    {
        if (keybinds.ContainsKey(actionName))
        {
            return keybinds[actionName];
        }
        Debug.LogError($"Action '{actionName}' not found in KeybindManager!");
        return KeyCode.None;
    }

    public static void ResetToDefaults()
    {
        // 1. Копируем значения по умолчанию в рабочий словарь
        keybinds = new Dictionary<string, KeyCode>(DefaultKeybinds);

        // 2. Сохраняем сброшенные значения в PlayerPrefs
        SaveKeybinds();

        // 3. НОВОЕ: Вызываем событие, чтобы обновить все кнопки
        OnKeybindsChanged?.Invoke();

        Debug.Log("Keybinds have been reset to default values.");
    }

    // Установить новую клавишу для определенного действия
    public static void SetKey(string actionName, KeyCode newKey)
    {
        if (keybinds.ContainsKey(actionName))
        {
            // 1. Обновляем привязку в словаре
            keybinds[actionName] = newKey;

            // 2. Сохраняем изменение
            SaveKeybinds();
        }
        else
        {
            Debug.LogError($"Cannot rebind. Action '{actionName}' not found.");
        }
    }

    // ----------------------------------------------------------------------
    // 3. Сохранение и Загрузка (Используем JSON для удобства)
    // ----------------------------------------------------------------------

    // Класс-контейнер для сериализации словаря
    [System.Serializable]
    private class KeybindsData
    {
        // Список, который хранит пары "ИмяДействия:КодКлавиши"
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();

        public void FromDictionary(Dictionary<string, KeyCode> dict)
        {
            keys.Clear();
            values.Clear();
            foreach (var pair in dict)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value.ToString());
            }
        }

        public Dictionary<string, KeyCode> ToDictionary()
        {
            Dictionary<string, KeyCode> dict = new Dictionary<string, KeyCode>();
            for (int i = 0; i < keys.Count; i++)
            {
                // Попытка преобразовать строку в KeyCode
                if (System.Enum.TryParse(values[i], out KeyCode keyCode))
                {
                    dict.Add(keys[i], keyCode);
                }
                else
                {
                    Debug.LogError($"Failed to parse KeyCode for action: {keys[i]} with value: {values[i]}");
                }
            }
            return dict;
        }
    }

    private static void SaveKeybinds()
    {
        KeybindsData data = new KeybindsData();
        data.FromDictionary(keybinds);

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KeybindsSaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Keybinds saved successfully.");
    }

    private static void LoadKeybinds()
    {
        if (PlayerPrefs.HasKey(KeybindsSaveKey))
        {
            string json = PlayerPrefs.GetString(KeybindsSaveKey);
            KeybindsData data = JsonUtility.FromJson<KeybindsData>(json);

            // Загружаем данные из JSON в наш рабочий словарь
            keybinds = data.ToDictionary();
            Debug.Log("Keybinds loaded successfully.");
        }
    }


}
