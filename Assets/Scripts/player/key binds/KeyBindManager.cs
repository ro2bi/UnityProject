using UnityEngine;
using System.Collections.Generic;

public static class KeybindManager
{
    // Имя, используемое для сохранения данных в PlayerPrefs
    private const string KeybindsSaveKey = "GameKeybinds";

    // Словарь, хранящий текущие привязки: ActionName -> KeyCode
    private static Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    // КОНСТАНТЫ ДЕЙСТВИЙ: используйте эти строки в RebindButton.ActionToRebind
    public const string JUMP = "Jump";
    public const string MOVE_RIGHT = "MoveRight";
    public const string MOVE_LEFT = "MoveLeft";
    public const string ATTACK = "Attack";
    // Добавьте сюда любые другие действия

    // ----------------------------------------------------------------------
    // 1. Инициализация и загрузка (Вызывается один раз при старте игры/меню)
    // ----------------------------------------------------------------------
    public static void InitializeKeys()
    {
        // 1. Определяем привязки по умолчанию
        Dictionary<string, KeyCode> defaultKeybinds = new Dictionary<string, KeyCode>
        {
            { JUMP, KeyCode.Space },
            { MOVE_RIGHT, KeyCode.D },
            { MOVE_LEFT, KeyCode.A },
            { ATTACK, KeyCode.E }
        };

        // 2. Загружаем сохраненные данные, если они есть
        if (PlayerPrefs.HasKey(KeybindsSaveKey))
        {
            LoadKeybinds();
        }
        else
        {
            // Используем значения по умолчанию, если нет сохраненных
            keybinds = defaultKeybinds;
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
