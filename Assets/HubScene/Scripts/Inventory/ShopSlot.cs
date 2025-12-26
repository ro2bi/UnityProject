using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI компоненты")]
    public Image itemIcon;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI inventoryCountText;

    [Header("Настройки цвета")]
    public Color affordableColor = Color.green;
    public Color expensiveColor = Color.red;

    private ItemData item;

    public void Initialize(ItemData itemData)
    {
        item = itemData;

        if (item != null)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = item.icon;
                itemIcon.enabled = true;
            }

            UpdateSlotUI();
        }
    }

    // Метод для обновления всех текстовых данных в слоте
    public void UpdateSlotUI()
    {
        if (item == null) return;

        // Обновляем количество в инвентаре
        UpdateInventoryCount();

        // Обновляем цену и её цвет
        if (priceText != null)
        {
            priceText.text = $"{item.buyPrice}";

            // Проверка: хватает ли денег
            if (InventorySystem.Instance != null)
            {
                bool canAfford = InventorySystem.Instance.CurrentMoney >= item.buyPrice;
                priceText.color = canAfford ? affordableColor : expensiveColor;
            }
        }
    }

    public void UpdateInventoryCount()
    {
        if (item != null && inventoryCountText != null && InventorySystem.Instance != null)
        {
            int count = InventorySystem.Instance.GetItemCount(item);
            inventoryCountText.text = $"{count}";
        }
    }

    // Наведение мыши
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Добавлена проверка на null для безопасности
        if (item != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.ShowTooltip(item, false);
        }
    }

    // Уход мыши
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnBuyButtonClicked();
        }
    }

    public void OnBuyButtonClicked()
    {
        if (item != null && InventorySystem.Instance != null)
        {
            bool success = InventorySystem.Instance.BuyItem(item);

            if (success)
            {
                Debug.Log($"Куплен предмет: {item.itemName}");

                // Сразу обновляем весь UI магазина, чтобы пересчитать цвета цен у ВСЕХ предметов
                UIManagerShop.Instance.RefreshShopUI();

                if (ItemTooltip.Instance != null)
                {
                    ItemTooltip.Instance.ShowTooltip(item, false);
                }
            }
            else
            {
                Debug.Log($"Недостаточно денег для покупки: {item.itemName}");
            }
        }
    }
}