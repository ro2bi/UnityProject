using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static ItemTooltip Instance { get; private set; }

    [Header("UI Текстовые компоненты")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI inventoryCountText;

    [Header("Панель и Кнопки")]
    public GameObject tooltipPanel;
    public Button buyButton;
    public Button sellButton;
    public Button dropButton;

    private ItemData currentItem;
    private bool isInInventory;
    public bool IsMouseOver { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Подписка на кнопки
        if (buyButton) buyButton.onClick.AddListener(OnBuyClicked);
        if (sellButton) sellButton.onClick.AddListener(OnSellClicked);
        if (dropButton) dropButton.onClick.AddListener(OnDropClicked);

        HideTooltip();
    }

    public void ShowTooltip(ItemData item, bool inInventory, Vector3 slotPosition = default)
    {
        if (item == null) return;
        currentItem = item;
        isInInventory = inInventory;

        // 1. Заполняем тексты (ТО, ЧТО НЕ РАБОТАЛО)
        if (itemNameText) itemNameText.text = item.itemName;
        if (descriptionText) descriptionText.text = item.description;

        // 2. Логика цен и количества
        int count = InventorySystem.Instance.GetItemCount(item);

        if (inInventory)
        {
            if (priceText) priceText.text = $"Цена продажи: {item.sellPrice}";
            if (inventoryCountText) inventoryCountText.text = $"У вас есть: {count}";

            if (buyButton) buyButton.gameObject.SetActive(false);
            if (sellButton) sellButton.gameObject.SetActive(true);
            if (dropButton) dropButton.gameObject.SetActive(true);
        }
        else
        {
            if (priceText) priceText.text = $"Цена покупки: {item.buyPrice}";
            if (inventoryCountText) inventoryCountText.text = $"В инвентаре: {count}";

            if (buyButton) buyButton.gameObject.SetActive(true);
            if (sellButton) sellButton.gameObject.SetActive(false);
            if (dropButton) dropButton.gameObject.SetActive(false);
        }

        tooltipPanel.SetActive(true);

        // 3. Позиционирование
        if (slotPosition != default)
        {
            // Смещение вправо (130) и чуть вверх (50), чтобы мышка не перекрывала
            Vector3 offset = new Vector3(130, 50, 0);
            tooltipPanel.transform.position = slotPosition + offset;
        }
    }

    // Метод для обновления данных без переоткрытия (например, после покупки)
    private void RefreshUI()
    {
        if (currentItem != null)
            ShowTooltip(currentItem, isInInventory, tooltipPanel.transform.position - new Vector3(130, 50, 0));
    }

    public void HideTooltip()
    {
        if (!IsMouseOver) tooltipPanel.SetActive(false);
    }

    public void ForceHide()
    {
        IsMouseOver = false;
        tooltipPanel.SetActive(false);
    }

    // --- ЛОГИКА КНОПОК ---
    private void OnBuyClicked()
    {
        if (currentItem != null && InventorySystem.Instance.BuyItem(currentItem))
        {
            RefreshUI(); // Сразу обновляем текст "В инвентаре: Х"
        }
    }

    private void OnSellClicked()
    {
        if (currentItem != null)
        {
            InventorySystem.Instance.SellItem(currentItem);
            ForceHide(); // Предмет исчез из слота, скрываем тултип
        }
    }

    private void OnDropClicked()
    {
        if (currentItem != null)
        {
            InventorySystem.Instance.DropItem(currentItem);
            ForceHide();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => IsMouseOver = true;
    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
        HideTooltip();
    }
}