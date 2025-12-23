using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

    [Header("UI компоненты")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI inventoryCountText;

    [Header("Кнопки")]
    public GameObject buyButton;
    public GameObject sellButton;
    public Button buyButtonComponent;
    public Button sellButtonComponent;

    private RectTransform tooltipRect;
    private Canvas canvas;
    private ItemData currentItem;
    private bool isInInventory;

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

        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Подписываемся на кнопки
        buyButtonComponent.onClick.AddListener(OnBuyClicked);
        sellButtonComponent.onClick.AddListener(OnSellClicked);

        HideTooltip();
    }

    public void ShowTooltip(ItemData item, bool inInventory)
    {
        if (item == null) return;

        currentItem = item;
        isInInventory = inInventory;

        // Заполняем информацию
        itemNameText.text = item.itemName;
        descriptionText.text = item.description;

        // Цены
        string priceInfo = $"Покупка: {item.buyPrice} серебра\nПродажа: {item.sellPrice} серебра";
        priceText.text = priceInfo;

        // Показываем соответствующие кнопки
        if (inInventory)
        {
            // В инвентаре: можно продать и купить еще
            buyButton.SetActive(true);
            sellButton.SetActive(true);
            inventoryCountText.gameObject.SetActive(false);
        }
        else
        {
            // В магазине: можно только купить, показываем количество в инвентаре
            buyButton.SetActive(true);
            sellButton.SetActive(false);

            int count = InventorySystem.Instance.GetItemCount(item);
            inventoryCountText.text = $"В инвентаре: {count}";
            inventoryCountText.gameObject.SetActive(true);
        }

        tooltipPanel.SetActive(true);

        // Позиционируем тултип возле курсора
        UpdatePosition();
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        currentItem = null;
    }

    private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;

        // Смещение от курсора
        Vector2 offset = new Vector2(20, -20);
        Vector2 targetPos = mousePos + offset;

        // Проверяем, чтобы тултип не выходил за границы экрана
        float tooltipWidth = tooltipRect.rect.width;
        float tooltipHeight = tooltipRect.rect.height;

        if (targetPos.x + tooltipWidth > Screen.width)
            targetPos.x = mousePos.x - tooltipWidth - 20;

        if (targetPos.y - tooltipHeight < 0)
            targetPos.y = mousePos.y + tooltipHeight + 20;

        tooltipPanel.transform.position = targetPos;
    }

    private void OnBuyClicked()
    {
        if (currentItem == null) return;

        bool success = InventorySystem.Instance.BuyItem(currentItem);
        if (success)
        {
            Debug.Log($"Куплен предмет: {currentItem.itemName}");

            // Обновляем отображение количества в магазине
            if (!isInInventory)
            {
                int count = InventorySystem.Instance.GetItemCount(currentItem);
                inventoryCountText.text = $"В инвентаре: {count}";
            }
        }
    }

    private void OnSellClicked()
    {
        if (currentItem == null) return;

        InventorySystem.Instance.SellItem(currentItem);
        Debug.Log($"Продан предмет: {currentItem.itemName}");
        HideTooltip();
    }
}
