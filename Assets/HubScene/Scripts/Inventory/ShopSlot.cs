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

    private ItemData item;

    public void Initialize(ItemData itemData)
    {
        item = itemData;

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;

            // Показываем цену
            if (priceText != null)
            {
                priceText.text = $"{item.buyPrice}";
            }

            UpdateInventoryCount();
        }
    }

    public void UpdateInventoryCount()
    {
        if (item != null && inventoryCountText != null)
        {
            int count = InventorySystem.Instance.GetItemCount(item);
            inventoryCountText.text = $"{count}";
        }
    }

    // Наведение мыши - показываем тултип
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ItemTooltip.Instance.ShowTooltip(item, false);
        }
    }

    // Уход мыши - скрываем тултип
    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    // Клик по слоту - покупка предмета
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnBuyButtonClicked();
        }
    }

    public void OnBuyButtonClicked()
    {
        if (item != null)
        {
            bool success = InventorySystem.Instance.BuyItem(item);

            if (success)
            {
                Debug.Log($"Куплен предмет: {item.itemName}");
                UpdateInventoryCount();

                // Обновляем тултип, чтобы показать новое количество
                ItemTooltip.Instance.ShowTooltip(item, false);
            }
            else
            {
                Debug.Log($"Недостаточно денег для покупки: {item.itemName}");
            }
        }
    }
}