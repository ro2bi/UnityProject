using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // Добавлено

public class UIManagerShop : MonoBehaviour
{
    public static UIManagerShop Instance { get; private set; }

    [Header("Главное окно")]
    public GameObject mainWindow;
    public GameObject inventoryPanel;
    public GameObject shopPanel;

    [Header("Кнопки вкладок")]
    public Button inventoryTabButton;
    public Button shopTabButton;

    [Header("Панель денег")]
    public TextMeshProUGUI moneyText;

    [Header("Сетки ИНВЕНТАРЯ по категориям")]
    public Transform invFoodGrid;
    public Transform invPotionsGrid;
    public Transform invClothingGrid;
    public GameObject inventorySlotPrefab; // префаб слота инвентаря

    [Header("Сетки МАГАЗИНА по категориям")]
    public Transform shopFoodGrid;
    public Transform shopPotionsGrid;
    public Transform shopClothingGrid;
    public GameObject shopSlotPrefab;

    private bool isInitialized = false;
    private bool isInventoryOpen = false;
    private bool isShopOpen = false;
    private bool escPressedThisFrame = false;

    public static bool IsWindowOpen => Instance != null && Instance.mainWindow.activeSelf;
    public static bool EscPressedThisFrame => Instance != null && Instance.escPressedThisFrame;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        KeybindManager.InitializeKeys();
    }

    private void Start()
    {
        mainWindow.SetActive(false);

        if (inventoryTabButton != null)
            inventoryTabButton.onClick.AddListener(() => SwitchTab(true));
        if (shopTabButton != null)
            shopTabButton.onClick.AddListener(() => SwitchTab(false));

        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            InventorySystem.Instance.OnInventoryChanged += RefreshInventoryUI;
        }

        StartCoroutine(InitializeUISystem());
    }

    private IEnumerator InitializeUISystem()
    {
        // Инвентарь создаём СРАЗУ
        CreateInventorySlots();

        if (InventorySystem.Instance != null)
        {
            UpdateMoneyDisplay(InventorySystem.Instance.CurrentMoney);
            RefreshInventoryUI();
        }

        // UI можно открывать
        isInitialized = true;

        // Ждём, пока магазин загрузится
        yield return new WaitUntil(() =>
            ShopSystem.Instance != null &&
            ShopSystem.Instance.AllItems != null &&
            ShopSystem.Instance.AllItems.Count > 0
        );

        Debug.Log($"Создаём слоты магазина: {ShopSystem.Instance.AllItems.Count}");

        CreateShopSlots();
    }

    private void Update()
    {
        if (!isInitialized) return;
        escPressedThisFrame = false;

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INVENTORY)))
        {
            if (isInventoryOpen) CloseWindow();
            else OpenWindow(true);
        }

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.OPEN_SHOP)))
        {
            if (isShopOpen) CloseWindow();
            else OpenWindow(false);
        }

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)) && mainWindow.activeSelf)
        {
            escPressedThisFrame = true;
            CloseWindow();
        }
    }

    // --- ЛОГИКА ИНВЕНТАРЯ ---

    private void RefreshInventoryUI()
    {
        if (InventorySystem.Instance == null) return;

        ClearInventorySlots();

        for (int i = 0; i < InventorySystem.Instance.Items.Count; i++)
        {
            ItemData item = InventorySystem.Instance.Items[i];
            if (item == null) continue;

            Transform grid = GetTargetGrid(item.itemType, true);
            if (grid == null) continue;

            InventorySlot freeSlot = GetFirstEmptySlot(grid);
            if (freeSlot != null)
            {
                freeSlot.slotIndex = i;
                freeSlot.SetItem(item);
            }
        }
    }

    private void ClearGrid(Transform grid)
    {
        if (grid == null) return;
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }

    // --- ЛОГИКА МАГАЗИНА ---

    private void CreateShopSlots()
    {
        if (ShopSystem.Instance == null) return;

        // Очистка обязательна, чтобы не дублировалось
        ClearGrid(shopFoodGrid);
        ClearGrid(shopPotionsGrid);
        ClearGrid(shopClothingGrid);

        int foodCount = 0;
        int potionCount = 0;
        int clothCount = 0;

        foreach (ItemData item in ShopSystem.Instance.AllItems)
        {
            if (item == null) continue;

            Transform targetGrid = null;

            if (item.itemType == ItemType.Food && foodCount < 5)
            {
                targetGrid = shopFoodGrid;
                foodCount++;
            }
            else if (item.itemType == ItemType.Potion && potionCount < 5)
            {
                targetGrid = shopPotionsGrid;
                potionCount++;
            }
            else if (item.itemType == ItemType.Clothing && clothCount < 5)
            {
                targetGrid = shopClothingGrid;
                clothCount++;
            }

            if (targetGrid != null)
            {
                GameObject slot = Instantiate(shopSlotPrefab, targetGrid);
                ShopSlot slotComponent = slot.GetComponent<ShopSlot>();
                if (slotComponent != null)
                {
                    slotComponent.Initialize(item);
                }
            }
        }

        // ВОТ ЭТОТ ЛОГ ДОЛЖЕН БЫТЬ ТУТ
        Debug.Log($"Магазин заполнен! Еда: {foodCount}, Зелья: {potionCount}, Одежда: {clothCount}");
    }

    // Универсальный помощник для выбора грида
    private Transform GetTargetGrid(ItemType type, bool isInventory)
    {
        switch (type)
        {
            case ItemType.Food:
                return isInventory ? invFoodGrid : shopFoodGrid;
            case ItemType.Potion:
                return isInventory ? invPotionsGrid : shopPotionsGrid;
            case ItemType.Clothing:
                return isInventory ? invClothingGrid : shopClothingGrid;
            default:
                return null;
        }
    }

    // --- ОСТАЛЬНЫЕ МЕТОДЫ (Окна, Деньги и т.д.) ---

    private void OpenWindow(bool openInventory)
    {
        mainWindow.SetActive(true);
        SwitchTab(openInventory);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseWindow()
    {
        mainWindow.SetActive(false);
        isInventoryOpen = false;
        isShopOpen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchTab(bool showInventory)
    {
        inventoryPanel.SetActive(showInventory);
        shopPanel.SetActive(!showInventory);
        isInventoryOpen = showInventory;
        isShopOpen = !showInventory;

        if (showInventory) RefreshInventoryUI();
        else RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        RefreshGridSlots(shopFoodGrid);
        RefreshGridSlots(shopPotionsGrid);
        RefreshGridSlots(shopClothingGrid);
    }

    private void RefreshGridSlots(Transform grid)
    {
        if (grid == null) return;
        foreach (Transform child in grid)
        {
            ShopSlot slot = child.GetComponent<ShopSlot>();
            if (slot != null) slot.UpdateInventoryCount();
        }
    }

    private void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null) moneyText.text = $"{money} серебра";
    }


    private void CreateInventorySlots()
    {
        CreateFixedSlots(invFoodGrid);
        CreateFixedSlots(invPotionsGrid);
        CreateFixedSlots(invClothingGrid);
    }

    private void CreateFixedSlots(Transform grid)
    {
        if (grid == null) return;

        foreach (Transform child in grid)
            Destroy(child.gameObject);

        int slotsPerCategory = InventorySystem.Instance.maxSlots / 3;

        for (int i = 0; i < slotsPerCategory; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, grid);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot != null)
                slot.ClearSlot(); // ВАЖНО: сразу пустой
        }
    }

    private void ClearInventorySlots()
    {
        ClearSlots(invFoodGrid);
        ClearSlots(invPotionsGrid);
        ClearSlots(invClothingGrid);
    }

    private void ClearSlots(Transform grid)
    {
        if (grid == null) return;

        foreach (Transform child in grid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
                slot.ClearSlot();
        }
    }

    private InventorySlot GetFirstEmptySlot(Transform grid)
    {
        foreach (Transform child in grid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.IsEmpty)
                return slot;
        }
        return null;
    }
}