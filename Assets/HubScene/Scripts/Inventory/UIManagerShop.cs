using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    private bool isInitialized = false;

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

        // Подписываемся на события кнопок
        if (inventoryTabButton != null)
            inventoryTabButton.onClick.AddListener(() => SwitchTab(true));
        if (shopTabButton != null)
            shopTabButton.onClick.AddListener(() => SwitchTab(false));

        // Подписываемся на события системы
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            InventorySystem.Instance.OnInventoryChanged += RefreshInventoryUI;
        }

        // Инициализация через корутину
        StartCoroutine(InitializeUISystem());
    }

    private IEnumerator InitializeUISystem()
    {
        // Создаем слоты
        CreateInventorySlots();
        CreateShopSlots();

        // Ждем 2 кадра для полной инициализации
        yield return null;
        yield return null;

        // Обновляем UI
        if (InventorySystem.Instance != null)
        {
            UpdateMoneyDisplay(InventorySystem.Instance.CurrentMoney);
            RefreshInventoryUI();
        }

        isInitialized = true;
        Debug.Log("UI система инициализирована");
    }

    private void Update()
    {
        if (!isInitialized) return;

        // сброс флага каждый кадр
        escPressedThisFrame = false;

        // Инвентарь
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INVENTORY)))
        {
            if (isInventoryOpen)
                CloseWindow();
            else
                OpenWindow(true);
        }

        // Магазин
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.OPEN_SHOP)))
        {
            if (isShopOpen)
                CloseWindow();
            else
                OpenWindow(false);
        }

        // ESC — закрываем инвентарь/магазин
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.TOMENU)) && mainWindow.activeSelf)
        {
            escPressedThisFrame = true;
            CloseWindow();
        }
    }

    private void OpenWindow(bool openInventory)
    {
        mainWindow.SetActive(true);
        SwitchTab(openInventory);
        PauseGame();

        // Разблокируем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseWindow()
    {
        mainWindow.SetActive(false);
        isInventoryOpen = false;
        isShopOpen = false;
        ResumeGame();

        // Блокируем курсор обратно (если нужно для вашей игры)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Игра на паузе (инвентарь/магазин открыт)");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Игра возобновлена (инвентарь/магазин закрыт)");
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
        if (inventoryGrid == null || slotPrefab == null || InventorySystem.Instance == null)
        {
            Debug.LogError("Не назначены необходимые компоненты для инвентаря!");
            return;
        }

        for (int i = 0; i < InventorySystem.Instance.maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryGrid);
            InventorySlot slotComponent = slot.GetComponent<InventorySlot>();
            if (slotComponent != null)
            {
                slotComponent.slotIndex = i;
            }
        }
    }

    private void CreateShopSlots()
    {
        if (shopGrid == null || shopSlotPrefab == null || ShopSystem.Instance == null)
        {
            Debug.LogError("Не назначены необходимые компоненты для магазина!");
            return;
        }

        foreach (ItemData item in ShopSystem.Instance.AllItems)
        {
            GameObject slot = Instantiate(shopSlotPrefab, shopGrid);
            ShopSlot slotComponent = slot.GetComponent<ShopSlot>();
            if (slotComponent != null)
            {
                slotComponent.Initialize(item);
            }
        }
    }

    private void RefreshInventoryUI()
    {
        if (!isInitialized || inventoryGrid == null || inventoryGrid.childCount == 0 || InventorySystem.Instance == null)
            return;

        for (int i = 0; i < inventoryGrid.childCount; i++)
        {
            Transform child = inventoryGrid.GetChild(i);
            if (child == null) continue;

            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot == null) continue;

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
        if (!isInitialized || shopGrid == null || shopGrid.childCount == 0)
            return;

        foreach (Transform child in shopGrid)
        {
            if (child == null) continue;

            ShopSlot slot = child.GetComponent<ShopSlot>();
            if (slot != null)
            {
                slot.UpdateInventoryCount();
            }
        }
    }

    private void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money} серебра";
        }
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