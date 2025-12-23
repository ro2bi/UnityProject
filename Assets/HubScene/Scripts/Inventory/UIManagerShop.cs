using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Сетка инвентаря")]
    public Transform inventoryGrid;
    public GameObject slotPrefab;

    [Header("Сетка магазина")]
    public Transform shopGrid;
    public GameObject shopSlotPrefab;

    private bool isInventoryOpen = false;
    private bool isShopOpen = false;
    private bool escPressedThisFrame = false;

    // Публичное свойство для проверки извне
    public static bool IsWindowOpen => Instance != null && Instance.mainWindow.activeSelf;
    public static bool EscPressedThisFrame => Instance != null && Instance.escPressedThisFrame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Инициализация KeybindManager
        KeybindManager.InitializeKeys();
    }

    private void Start()
    {
        // Закрываем окно при старте
        mainWindow.SetActive(false);

        // Подписываемся на события
        inventoryTabButton.onClick.AddListener(() => SwitchTab(true));
        shopTabButton.onClick.AddListener(() => SwitchTab(false));

        InventorySystem.Instance.OnMoneyChanged += UpdateMoneyDisplay;
        InventorySystem.Instance.OnInventoryChanged += RefreshInventoryUI;

        // Создаем слоты
        CreateInventorySlots();
        CreateShopSlots();

        // Обновляем UI
        UpdateMoneyDisplay(InventorySystem.Instance.CurrentMoney);
        RefreshInventoryUI();
    }

    private void Update()
    {
        // Проверка нажатия клавиши инвентаря
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INVENTORY)))
        {
            if (isInventoryOpen)
            {
                // Если инвентарь уже открыт - закрываем окно
                CloseWindow();
            }
            else
            {
                // Если инвентарь не открыт - открываем его
                OpenWindow(true);
            }
        }

        // Проверка нажатия клавиши магазина
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.OPEN_SHOP)))
        {
            if (isShopOpen)
            {
                // Если магазин уже открыт - закрываем окно
                CloseWindow();
            }
            else
            {
                // Если магазин не открыт - открываем его
                OpenWindow(false);
            }
        }

        // ESC для закрытия инвентаря/магазина (без открытия паузы)
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)) && mainWindow.activeSelf)
        {
            CloseWindow();
        }
    }

    private void OpenWindow(bool openInventory)
    {
        mainWindow.SetActive(true);
        SwitchTab(openInventory);
        PauseGame();
    }

    private void CloseWindow()
    {
        mainWindow.SetActive(false);
        isInventoryOpen = false;
        isShopOpen = false;
        ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Игра на паузе (инвентарь/магазин)");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Игра возобновлена");
    }

    private void SwitchTab(bool showInventory)
    {
        if (showInventory)
        {
            inventoryPanel.SetActive(true);
            shopPanel.SetActive(false);
            isInventoryOpen = true;
            isShopOpen = false;
            RefreshInventoryUI();
        }
        else
        {
            inventoryPanel.SetActive(false);
            shopPanel.SetActive(true);
            isInventoryOpen = false;
            isShopOpen = true;
            RefreshShopUI();
        }
    }

    private void CreateInventorySlots()
    {
        for (int i = 0; i < InventorySystem.Instance.maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryGrid);
            InventorySlot slotComponent = slot.GetComponent<InventorySlot>();
            slotComponent.slotIndex = i;
        }
    }

    private void CreateShopSlots()
    {
        foreach (ItemData item in ShopSystem.Instance.AllItems)
        {
            GameObject slot = Instantiate(shopSlotPrefab, shopGrid);
            ShopSlot slotComponent = slot.GetComponent<ShopSlot>();
            slotComponent.Initialize(item);
        }
    }

    private void RefreshInventoryUI()
    {
        for (int i = 0; i < inventoryGrid.childCount; i++)
        {
            InventorySlot slot = inventoryGrid.GetChild(i).GetComponent<InventorySlot>();
            if (i < InventorySystem.Instance.Items.Count)
            {
                slot.SetItem(InventorySystem.Instance.Items[i]);
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }

    private void RefreshShopUI()
    {
        foreach (Transform child in shopGrid)
        {
            ShopSlot slot = child.GetComponent<ShopSlot>();
            slot.UpdateInventoryCount();
        }
    }

    private void UpdateMoneyDisplay(int money)
    {
        moneyText.text = $"{money} серебра";
    }

    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            InventorySystem.Instance.OnInventoryChanged -= RefreshInventoryUI;
        }
    }
}