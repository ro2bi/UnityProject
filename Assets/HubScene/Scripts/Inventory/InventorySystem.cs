using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Настройки")]
    public int maxSlots = 15;
    public int startingMoney = 100;
    public GameObject worldItemPrefab;

    [Header("Экипировка персонажа")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer legsRenderer;

    private List<ItemData> items = new List<ItemData>();
    private List<string> pickedUpItemIDs = new List<string>();
    private int currentMoney;

    public event Action OnInventoryChanged;
    public event Action<int> OnMoneyChanged;

    public int CurrentMoney => currentMoney;
    public List<ItemData> Items => items;

    [Header("Экипировка")]
    public SpriteRenderer handRenderer; // Создай на игроке дочерний объект Hand и перетащи сюда

    private ItemData equippedTool; // Храним текущий инструмент (кирку)

    public void EquipItem(ItemData item)
    {
        if (item == null) return;

        if (item.itemType == ItemType.Tool)
        {
            equippedTool = item;
            if (handRenderer != null)
            {
                handRenderer.sprite = item.equipSprite; 
                handRenderer.gameObject.SetActive(true);

                float s = item.worldScale;
                handRenderer.transform.localScale = new Vector3(s, s, s);
            }
            Debug.Log("Экипирован инструмент: " + item.itemName);
        }
        else if (item.itemType == ItemType.Clothing)
        {
            if (item.itemName.Contains("Hat") && headRenderer) headRenderer.sprite = item.equipSprite;
            if (item.itemName.Contains("Shirt") && bodyRenderer) bodyRenderer.sprite = item.equipSprite;
            if (item.itemName.Contains("Pants") && legsRenderer) legsRenderer.sprite = item.equipSprite;

            Debug.Log("Надета одежда: " + item.itemName);
        }
        else if (item.isUsable)
        {
            UseItem(item);
        }

        OnInventoryChanged?.Invoke();
    }

    public ItemData GetEquippedTool() => equippedTool;


    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void Start() { LoadInventory(); }

    public bool WasItemPickedUp(string id) => pickedUpItemIDs.Contains(id);

    public void RegisterPickedUpItem(string id)
    {
        if (!string.IsNullOrEmpty(id) && !pickedUpItemIDs.Contains(id))
        {
            pickedUpItemIDs.Add(id);
            SaveInventory();
        }
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots) return false;
        items.Add(item);
        OnInventoryChanged?.Invoke();
        SaveInventory();
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= items.Count || toIndex < 0 || toIndex >= maxSlots) return;
        ItemData temp = items[fromIndex];
        items.RemoveAt(fromIndex);
        if (toIndex >= items.Count) items.Add(temp);
        else items.Insert(toIndex, temp);
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    public int GetItemCount(ItemData item) => items.Count(i => i == item);

    public void DropItem(ItemData item)
    {
        if (item == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Если игрок не найден, спавним в центре, если найден - чуть сбоку
        Vector3 spawnPos = player ? player.transform.position + new Vector3(1f, 0, 0) : Vector3.zero;

        // ПРИНУДИТЕЛЬНО ставим Z как у игрока, чтобы предмет был виден
        if (player) spawnPos.z = player.transform.position.z;

        if (worldItemPrefab != null)
        {
            GameObject dropped = Instantiate(worldItemPrefab, spawnPos, Quaternion.identity);
            WorldItem wItem = dropped.GetComponent<WorldItem>();
            if (wItem != null)
            {
                wItem.SetItem(item, 1);
                Debug.Log($"Предмет {item.itemName} создан в {spawnPos}");
            }
        }
        else
        {
            Debug.LogError("Префаб WorldItem не назначен в InventorySystem!");
        }

        RemoveItem(item);
    }

    public bool BuyItem(ItemData item)
    {
        if (currentMoney >= item.buyPrice && AddItem(item))
        {
            AddMoney(-item.buyPrice);
            return true;
        }
        return false;
    }

    public void SellItem(ItemData item) { RemoveItem(item); AddMoney(item.sellPrice); }
    public void AddMoney(int amount) { currentMoney += amount; OnMoneyChanged?.Invoke(currentMoney); SaveInventory(); }

    public void UseItem(ItemData item)
    {
        if (item == null || !item.isUsable) return;
        Debug.Log("Использован: " + item.itemName);
        // Тут твоя логика баффов из старого скрипта...
        RemoveItem(item);
    }

    private void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData();
        data.money = currentMoney;
        data.itemNames = items.Select(i => i.name).ToList();
        data.pickedUpIDs = pickedUpItemIDs;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string json = PlayerPrefs.GetString("InventoryData");
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            currentMoney = data.money;
            pickedUpItemIDs = data.pickedUpIDs ?? new List<string>();
            items.Clear();
            foreach (var name in data.itemNames)
            {
                ItemData asset = Resources.Load<ItemData>($"Items/{name}");
                if (asset) items.Add(asset);
            }
        }
        else { currentMoney = startingMoney; }
        OnInventoryChanged?.Invoke();
        OnMoneyChanged?.Invoke(currentMoney);
    }
}

[Serializable]
public class InventorySaveData
{
    public int money;
    public List<string> itemNames;
    public List<string> pickedUpIDs;
}