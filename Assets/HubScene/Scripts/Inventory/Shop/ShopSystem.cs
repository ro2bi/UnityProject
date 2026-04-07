using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [Header("Список всех предметов в игре")]
    public List<ItemData> allItems = new List<ItemData>();

    [Header("Настройки загрузки")]
    [Tooltip("Если true, автоматически загрузит все предметы из Resources/Items")]
    public bool autoLoadFromResources = true;

    public List<ItemData> AllItems => allItems;

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
        if (autoLoadFromResources)
        {
            LoadAllItems();
        }

        if (allItems.Count == 0)
        {
            Debug.LogWarning("⚠ShopSystem: Нет предметов для продажи! Добавьте предметы в список или включите autoLoadFromResources.");
        }
    }

    private void LoadAllItems()
    {
        ItemData[] items = Resources.LoadAll<ItemData>("Items");

        if (items.Length == 0)
        {
            Debug.LogWarning("⚠ShopSystem: Не найдено предметов в Resources/Items!");
            return;
        }

        allItems.Clear();
        allItems.AddRange(items);

        Debug.Log($"Загружено {allItems.Count} предметов в магазин");
    }

    public List<ItemData> GetItemsByType(ItemType type)
    {
        List<ItemData> result = new List<ItemData>();

        foreach (ItemData item in allItems)
        {
            if (item.itemType == type)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public List<ItemData> GetClothingItems()
    {
        return GetItemsByType(ItemType.Clothing);
    }

    public List<ItemData> GetFoodItems()
    {
        return GetItemsByType(ItemType.Food);
    }
}