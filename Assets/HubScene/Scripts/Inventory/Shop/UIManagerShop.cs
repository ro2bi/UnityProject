using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
    private GameObject invTabObj;
    private GameObject shopTabObj;

    [Header("Панель денег")]
    public TextMeshProUGUI moneyText;

    [Header("Сетки ИНВЕНТАРЯ")]
    public Transform invFoodGrid;
    public Transform invPotionsGrid;
    public Transform invClothingGrid;
    public GameObject inventorySlotPrefab;

    [Header("Сетки МАГАЗИНА")]
    public Transform shopFoodGrid;
    public Transform shopPotionsGrid;
    public Transform shopClothingGrid;
    public GameObject shopSlotPrefab;

    private bool isInitialized = false;
    private bool isInventoryOpen = false;
    private bool isShopOpen = false;
    private bool escPressedThisFrame = false;

    // Свойства для обращения из других скриптов
    public static bool IsWindowOpen => Instance != null && Instance.mainWindow.activeSelf;
    public static bool EscPressedThisFrame => Instance != null && Instance.escPressedThisFrame;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        KeybindManager.InitializeKeys();

        if (inventoryTabButton != null) invTabObj = inventoryTabButton.gameObject;
        if (shopTabButton != null) shopTabObj = shopTabButton.gameObject;
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
        CreateInventorySlots();
        if (InventorySystem.Instance != null)
        {
            UpdateMoneyDisplay(InventorySystem.Instance.CurrentMoney);
            RefreshInventoryUI();
        }
        isInitialized = true;
        yield return null;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // Сбрасываем флаг в начале каждого кадра
        escPressedThisFrame = false;

        // Открытие инвентаря игроком
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INVENTORY)))
        {
            if (mainWindow.activeSelf) CloseWindow();
            else OpenJustInventory();
        }

        // Закрытие на ESC
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)) && mainWindow.activeSelf)
        {
            escPressedThisFrame = true; // Устанавливаем флаг перед закрытием
            CloseWindow();
        }
    }

    // --- МЕТОДЫ ОТКРЫТИЯ ---

    public void OpenJustInventory()
    {
        mainWindow.SetActive(true);
        if (invTabObj) invTabObj.SetActive(false);
        if (shopTabObj) shopTabObj.SetActive(false);

        SwitchTab(true);
        SetUIState(true);
    }

    public void OpenMerchantShop(List<ItemData> merchantItems)
    {
        PopulateShopSlots(merchantItems);
        mainWindow.SetActive(true);
        if (invTabObj) invTabObj.SetActive(true);
        if (shopTabObj) shopTabObj.SetActive(true);

        SwitchTab(false);
        SetUIState(true);
    }

    private void SetUIState(bool isOpen)
    {
        if (isOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void CloseWindow()
    {
        mainWindow.SetActive(false);
        isInventoryOpen = false;
        isShopOpen = false;
        SetUIState(false);
    }

    // --- ЛОГИКА ОБНОВЛЕНИЯ UI ---

    private void SwitchTab(bool showInventory)
    {
        inventoryPanel.SetActive(showInventory);
        shopPanel.SetActive(!showInventory);
        isInventoryOpen = showInventory;
        isShopOpen = !showInventory;

        if (showInventory) RefreshInventoryUI();
        else RefreshShopUI();
    }

    private void PopulateShopSlots(List<ItemData> items)
    {
        ClearGrid(shopFoodGrid);
        ClearGrid(shopPotionsGrid);
        ClearGrid(shopClothingGrid);

        foreach (ItemData item in items)
        {
            if (item == null) continue;
            Transform targetGrid = GetTargetGrid(item.itemType, false);
            if (targetGrid != null)
            {
                GameObject slot = Instantiate(shopSlotPrefab, targetGrid);
                ShopSlot slotComponent = slot.GetComponent<ShopSlot>();
                if (slotComponent != null) slotComponent.Initialize(item);
            }
        }
    }

    private void RefreshInventoryUI()
    {
        if (InventorySystem.Instance == null) return;
        ClearInventorySlots();
        for (int i = 0; i < InventorySystem.Instance.Items.Count; i++)
        {
            ItemData item = InventorySystem.Instance.Items[i];
            if (item == null) continue;
            Transform grid = GetTargetGrid(item.itemType, true);
            InventorySlot freeSlot = GetFirstEmptySlot(grid);
            if (freeSlot != null) { freeSlot.slotIndex = i; freeSlot.SetItem(item); }
        }
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

    private Transform GetTargetGrid(ItemType type, bool isInventory)
    {
        switch (type)
        {
            case ItemType.Food:
                return isInventory ? invFoodGrid : shopFoodGrid;

            case ItemType.Potion:
            case ItemType.Tool:
                return isInventory ? invPotionsGrid : shopPotionsGrid;

            case ItemType.Clothing:
                return isInventory ? invClothingGrid : shopClothingGrid;

            default:
                return null;
        }
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
        foreach (Transform child in grid) Destroy(child.gameObject);
        int slotsPerCategory = InventorySystem.Instance.maxSlots / 3;
        for (int i = 0; i < slotsPerCategory; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, grid);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot != null) slot.ClearSlot();
        }
    }

    private void ClearInventorySlots()
    {
        ClearSlots(invFoodGrid); ClearSlots(invPotionsGrid); ClearSlots(invClothingGrid);
    }

    private void ClearSlots(Transform grid)
    {
        if (grid == null) return;
        foreach (Transform child in grid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null) slot.ClearSlot();
        }
    }

    private void ClearGrid(Transform grid)
    {
        if (grid == null) return;
        foreach (Transform child in grid) Destroy(child.gameObject);
    }

    private InventorySlot GetFirstEmptySlot(Transform grid)
    {
        foreach (Transform child in grid)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.IsEmpty) return slot;
        }
        return null;
    }
}