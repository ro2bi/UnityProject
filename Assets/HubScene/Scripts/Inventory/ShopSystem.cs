using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [Header("Список всех предметов в игре")]
    public List<ItemData> allItems = new List<ItemData>();

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
        // Автоматически загрузить все предметы из папки Resources/Items
        LoadAllItems();
    }

    private void LoadAllItems()
    {
        if (allItems.Count == 0)
        {
            ItemData[] items = Resources.LoadAll<ItemData>("Items");
            allItems.AddRange(items);
            Debug.Log($"Загружено {allItems.Count} предметов в магазин");
        }
    }
}