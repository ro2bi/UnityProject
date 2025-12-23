using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI компоненты")]
    public Image itemIcon;
    public TextMeshProUGUI inventoryCountText;

    private ItemData item;

    public void Initialize(ItemData itemData)
    {
        item = itemData;

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            UpdateInventoryCount();
        }
    }

    public void UpdateInventoryCount()
    {
        if (item != null)
        {
            int count = InventorySystem.Instance.GetItemCount(item);
            inventoryCountText.text = $"В инвентаре: {count}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ItemTooltip.Instance.ShowTooltip(item, false);
            UpdateInventoryCount();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    public void OnBuyButtonClicked()
    {
        if (item != null)
        {
            bool success = InventorySystem.Instance.BuyItem(item);
            if (success)
            {
                UpdateInventoryCount();
                // Обновляем тултип, чтобы показать новое количество
                ItemTooltip.Instance.ShowTooltip(item, false);
            }
        }
    }
}
