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
    public GameObject inventorySlotPrefab; // Ваш префаб слота инвентаря

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
        // Магазин создается один раз при старте
        CreateShopSlots();

        yield return null;

        if (InventorySystem.Instance != null)
        {
            UpdateMoneyDisplay(InventorySystem.Instance.CurrentMoney);
            RefreshInventoryUI(); // Первичное заполнение инвентаря
        }

        isInitialized = true;
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

        // 1. Очищаем все старые слоты во всех гридах инвентаря
        ClearGrid(invFoodGrid);
        ClearGrid(invPotionsGrid);
        ClearGrid(invClothingGrid);

        // 2. Раскладываем предметы по категориям
        // Важно: проходим по списку предметов игрока
        for (int i = 0; i < InventorySystem.Instance.Items.Count; i++)
        {
            ItemData item = InventorySystem.Instance.Items[i];
            if (item == null) continue;

            Transform targetGrid = GetTargetGrid(item.itemType, true);

            if (targetGrid != null)
            {
                GameObject slotObj = Instantiate(inventorySlotPrefab, targetGrid);
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                if (slot != null)
                {
                    slot.slotIndex = i; // Сохраняем индекс для системы инвентаря
                    slot.SetItem(item);
                }
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

        foreach (ItemData item in ShopSystem.Instance.AllItems)
        {
            Transform targetGrid = GetTargetGrid(item.itemType, false);

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

    private void RefreshShopUI()
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
}