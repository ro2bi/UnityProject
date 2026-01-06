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
    public Button equipButton; // Твоя кнопка

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

        // НОВОЕ: Подписка на кнопку экипировки
        if (equipButton) equipButton.onClick.AddListener(OnEquipClicked);

        HideTooltip();
    }

    public void ShowTooltip(ItemData item, bool inInventory, Vector3 slotPosition = default)
    {
        if (item == null) return;
        currentItem = item;
        isInInventory = inInventory;

        if (itemNameText) itemNameText.text = item.itemName;
        if (descriptionText) descriptionText.text = item.description;

        int count = InventorySystem.Instance.GetItemCount(item);

        if (inInventory)
        {
            if (priceText) priceText.text = $"Sell: {item.sellPrice}";
            if (inventoryCountText) inventoryCountText.text = $"You have: {count}";

            if (buyButton) buyButton.gameObject.SetActive(false);
            if (sellButton) sellButton.gameObject.SetActive(true);
            if (dropButton) dropButton.gameObject.SetActive(true);

            // НОВОЕ: Показываем кнопку экипировки только для Одежды и Инструментов
            if (equipButton)
            {
                bool canEquip = item.itemType == ItemType.Clothing || item.itemType == ItemType.Tool;
                equipButton.gameObject.SetActive(canEquip);

                // Меняем текст кнопки для красоты
                var btnText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText) btnText.text = (item.itemType == ItemType.Clothing) ? "Надеть" : "Взять в руки";
            }
        }
        else
        {
            if (priceText) priceText.text = $"Buy: {item.buyPrice}";
            if (inventoryCountText) inventoryCountText.text = $"Avalible: {count}";

            if (buyButton) buyButton.gameObject.SetActive(true);
            if (sellButton) sellButton.gameObject.SetActive(false);
            if (dropButton) dropButton.gameObject.SetActive(false);
            if (equipButton) equipButton.gameObject.SetActive(false); // В магазине нельзя экипировать
        }

        tooltipPanel.SetActive(true);

        if (slotPosition != default)
        {
            Vector3 offset = new Vector3(130, 50, 0);
            tooltipPanel.transform.position = slotPosition + offset;
        }
    }

    // --- ЛОГИКА КНОПОК ---

    private void OnEquipClicked()
    {
        if (currentItem != null && isInInventory)
        {
            // Вызываем метод экипировки в системе инвентаря
            InventorySystem.Instance.EquipItem(currentItem);
            ForceHide(); // Закрываем тултип после выбора
        }
    }

    // Остальные методы без изменений...
    private void RefreshUI()
    {
        if (currentItem != null)
            ShowTooltip(currentItem, isInInventory, tooltipPanel.transform.position - new Vector3(130, 50, 0));
    }

    public void HideTooltip() { if (!IsMouseOver) tooltipPanel.SetActive(false); }
    public void ForceHide() { IsMouseOver = false; tooltipPanel.SetActive(false); }

    private void OnBuyClicked() { if (currentItem != null && InventorySystem.Instance.BuyItem(currentItem)) RefreshUI(); }
    private void OnSellClicked() { if (currentItem != null) { InventorySystem.Instance.SellItem(currentItem); ForceHide(); } }
    private void OnDropClicked() { if (currentItem != null) { InventorySystem.Instance.DropItem(currentItem); ForceHide(); } }
    public void OnPointerEnter(PointerEventData eventData) => IsMouseOver = true;
    public void OnPointerExit(PointerEventData eventData) { IsMouseOver = false; HideTooltip(); }
}