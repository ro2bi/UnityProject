using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Настройки")]
    public int maxSlots = 15;
    public int startingMoney = 100;

    [Header("Экипировка персонажа")]
    public SpriteRenderer headRenderer;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer legsRenderer;

    // Данные инвентаря
    private List<ItemData> items = new List<ItemData>();
    private int currentMoney;

    // Экипированные предметы
    private Dictionary<EquipmentSlot, ItemData> equippedItems = new Dictionary<EquipmentSlot, ItemData>();

    // События для обновления UI
    public event Action OnInventoryChanged;
    public event Action<int> OnMoneyChanged;

    public int CurrentMoney => currentMoney;
    public List<ItemData> Items => items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadInventory();
        
    }

    // Добавить предмет в инвентарь
    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Инвентарь полон!");
            return false;
        }

        items.Add(item);
        OnInventoryChanged?.Invoke();
        SaveInventory();
        return true;
    }

    // Удалить предмет из инвентаря
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    // Переместить предмет (для drag&drop)
    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= items.Count || toIndex < 0 || toIndex >= maxSlots)
            return;

        ItemData temp = items[fromIndex];
        items.RemoveAt(fromIndex);

        if (toIndex >= items.Count)
            items.Add(temp);
        else
            items.Insert(toIndex, temp);

        OnInventoryChanged?.Invoke();
        SaveInventory();
    }

    // Купить предмет
    public bool BuyItem(ItemData item)
    {
        if (currentMoney < item.buyPrice)
        {
            Debug.Log("Недостаточно денег!");
            return false;
        }

        if (AddItem(item))
        {
            AddMoney(-item.buyPrice);
            return true;
        }

        return false;
    }

    // Продать предмет
    public void SellItem(ItemData item)
    {
        RemoveItem(item);
        AddMoney(item.sellPrice);
    }

    // Использовать предмет
    public void UseItem(ItemData item)
    {
        if (item == null) return;
        if (!item.isUsable) return;

        PlayerMovementNew player =
            FindObjectOfType<PlayerMovementNew>();

        if (player == null) return;

        //ЕДА / ЗЕЛЬЯ
        if (item.itemType == ItemType.Food)
        {
            // тут пізніше hp / stamina
        }

        // БАФФ СТРИБКА
        if (item.jumpHeightBonus != 0 || item.jumpDurationBonus != 0)
        {
            player.ApplyJumpBuff(
                item.jumpHeightBonus,
                item.jumpDurationBonus,
                item.buffDuration
            );
        }

        // удаляем предмет (не стакается)
        RemoveItem(item);
    }

    // Экипировать одежду
    private void EquipClothing(ItemData item)
    {
        // Снимаем старую одежду в затронутых слотах
        if ((item.equipmentSlots & EquipmentSlot.Head) != 0)
        {
            UnequipSlot(EquipmentSlot.Head);
            equippedItems[EquipmentSlot.Head] = item;
            if (headRenderer != null)
                headRenderer.sprite = item.headSprite;
        }

        if ((item.equipmentSlots & EquipmentSlot.Body) != 0)
        {
            UnequipSlot(EquipmentSlot.Body);
            equippedItems[EquipmentSlot.Body] = item;
            if (bodyRenderer != null)
                bodyRenderer.sprite = item.bodySprite;
        }

        if ((item.equipmentSlots & EquipmentSlot.Legs) != 0)
        {
            UnequipSlot(EquipmentSlot.Legs);
            equippedItems[EquipmentSlot.Legs] = item;
            if (legsRenderer != null)
                legsRenderer.sprite = item.legsSprite;
        }

        Debug.Log($"Экипирована одежда: {item.itemName}");
        SaveInventory();
    }

    // Снять одежду со слота
    private void UnequipSlot(EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot))
        {
            equippedItems.Remove(slot);

            // Очищаем спрайт
            switch (slot)
            {
                case EquipmentSlot.Head:
                    if (headRenderer != null) headRenderer.sprite = null;
                    break;
                case EquipmentSlot.Body:
                    if (bodyRenderer != null) bodyRenderer.sprite = null;
                    break;
                case EquipmentSlot.Legs:
                    if (legsRenderer != null) legsRenderer.sprite = null;
                    break;
            }
        }
    }

    // Съесть еду
    private void ConsumeFood(ItemData item)
    {
        // TODO: Добавьте здесь логику восстановления здоровья/энергии
        // Например:
        // PlayerHealth.Instance.Heal(item.healthRestore);
        // PlayerEnergy.Instance.RestoreEnergy(item.energyRestore);

        Debug.Log($"Использована еда: {item.itemName}");
        Debug.Log($"Восстановление здоровья: {item.healthRestore}, энергии: {item.energyRestore}");

        RemoveItem(item);
    }

    // Проверить, сколько предметов определенного типа в инвентаре
    public int GetItemCount(ItemData item)
    {
        int count = 0;
        foreach (var invItem in items)
        {
            if (invItem == item)
                count++;
        }
        return count;
    }

    // Управление деньгами
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        SaveInventory();
    }

    // Сохранение инвентаря
    private void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            money = currentMoney,
            itemNames = new List<string>()
        };

        foreach (var item in items)
        {
            if (item != null)
                saveData.itemNames.Add(item.name);
        }

        // Сохранение экипированных предметов
        saveData.equippedItemNames = new Dictionary<string, string>();
        foreach (var kvp in equippedItems)
        {
            if (kvp.Value != null)
                saveData.equippedItemNames[kvp.Key.ToString()] = kvp.Value.name;
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    // Загрузка инвентаря
    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string json = PlayerPrefs.GetString("InventoryData");
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            currentMoney = saveData.money;
            items.Clear();

            foreach (string itemName in saveData.itemNames)
            {
                ItemData item = Resources.Load<ItemData>($"Items/{itemName}");
                if (item != null)
                    items.Add(item);
            }

            // Загрузка экипированных предметов
            equippedItems.Clear();
            if (saveData.equippedItemNames != null)
            {
                foreach (var kvp in saveData.equippedItemNames)
                {
                    if (Enum.TryParse(kvp.Key, out EquipmentSlot slot))
                    {
                        ItemData item = Resources.Load<ItemData>($"Items/{kvp.Value}");
                        if (item != null)
                        {
                            equippedItems[slot] = item;
                            // Применяем визуал
                            if (slot == EquipmentSlot.Head && headRenderer != null)
                                headRenderer.sprite = item.headSprite;
                            if (slot == EquipmentSlot.Body && bodyRenderer != null)
                                bodyRenderer.sprite = item.bodySprite;
                            if (slot == EquipmentSlot.Legs && legsRenderer != null)
                                legsRenderer.sprite = item.legsSprite;
                        }
                    }
                }
            }

            OnMoneyChanged?.Invoke(currentMoney);
            OnInventoryChanged?.Invoke();
        }
        else
        {
            currentMoney = startingMoney;
            OnMoneyChanged?.Invoke(currentMoney);
        }
    }

    private void ApplyUseEffect(ItemData item)
    {
        if (item.useEffect == ItemUseEffect.None)
            return;

        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("PlayerStats not found");
            return;
        }

        switch (item.useEffect)
        {
            case ItemUseEffect.JumpBoost:
                PlayerStats.Instance.ApplyJumpBoost(
                    item.jumpHeightBonus,
                    item.jumpDurationBonus,
                    item.effectDuration
                );
                break;

            case ItemUseEffect.SpeedBoost:
                PlayerStats.Instance.ApplySpeedBoost(
                    item.speedBonus,
                    item.effectDuration
                );
                break;
        }
    }
}

[System.Serializable]
public class InventorySaveData
{
    public int money;
    public List<string> itemNames;
    public Dictionary<string, string> equippedItemNames;
}