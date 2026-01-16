using UnityEngine;
using System.Collections.Generic;
using System;

public static class KeybindManager
{
    private const string KeybindsSaveKey = "GameKeybinds";
    private static Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();
    public static event Action OnKeybindsChanged;

    // КОНСТАНТИ ДІЙ
    public const string JUMP = "Jump";
    public const string MOVE_FORWARD = "MoveForward";
    public const string MOVE_BACKWARD = "MoveBackward";
    public const string MOVE_RIGHT = "MoveRight";
    public const string MOVE_LEFT = "MoveLeft";
    public const string INTERACT = "Interact";
    public const string TOMENU = "ToMenu";
    public const string INVENTORY = "Inventory";
    public const string DROP = "Drop";
    public const string HELPWINDOW = "HelpWindow";

    private static readonly Dictionary<string, KeyCode> DefaultKeybinds = new Dictionary<string, KeyCode>
    {
        { JUMP, KeyCode.Space },
        { MOVE_FORWARD, KeyCode.W },
        { MOVE_BACKWARD, KeyCode.S },
        { MOVE_RIGHT, KeyCode.D },
        { MOVE_LEFT, KeyCode.A },
        { INTERACT, KeyCode.E },
        { TOMENU, KeyCode.Escape },
        { INVENTORY, KeyCode.R },
        { DROP, KeyCode.G },
        { HELPWINDOW, KeyCode.H },
    };

    public static void InitializeKeys()
    {
        if (PlayerPrefs.HasKey(KeybindsSaveKey))
        {
            LoadKeybinds();
            // ВАЖЛИВО: Перевіряємо, чи всі дії присутні після завантаження
            ValidateKeybinds();
        }
        else
        {
            keybinds = new Dictionary<string, KeyCode>(DefaultKeybinds);
            SaveKeybinds();
        }
    }

    // НОВИЙ МЕТОД: Перевіряє і додає відсутні дії
    private static void ValidateKeybinds()
    {
        bool needsSave = false;

        foreach (var defaultPair in DefaultKeybinds)
        {
            if (!keybinds.ContainsKey(defaultPair.Key))
            {
                keybinds.Add(defaultPair.Key, defaultPair.Value);
                needsSave = true;
                Debug.Log($"Added missing keybind: {defaultPair.Key} = {defaultPair.Value}");
            }
        }

        if (needsSave)
        {
            SaveKeybinds();
        }
    }

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
        keybinds = new Dictionary<string, KeyCode>(DefaultKeybinds);
        SaveKeybinds();
        OnKeybindsChanged?.Invoke();
        Debug.Log("Keybinds have been reset to default values.");
    }

    public static void SetKey(string actionName, KeyCode newKey)
    {
        if (keybinds.ContainsKey(actionName))
        {
            keybinds[actionName] = newKey;
            SaveKeybinds();
            OnKeybindsChanged?.Invoke();
        }
        else
        {
            Debug.LogError($"Cannot rebind. Action '{actionName}' not found.");
        }
    }

    [System.Serializable]
    private class KeybindsData
    {
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
            keybinds = data.ToDictionary();
            Debug.Log("Keybinds loaded successfully.");
        }
    }

    public static Dictionary<string, KeyCode> GetAllKeybinds()
    {
        // Возвращаем копию словаря, чтобы UI мог его перебрать
        return new Dictionary<string, KeyCode>(keybinds);
    }
}