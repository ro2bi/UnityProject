using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

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

    public void UpdateSlotUI()
    {
        if (item == null) return;

        UpdateInventoryCount();

        if (priceText != null)
        {
            priceText.text = $"{item.buyPrice}";
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

    private Coroutine delayCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            delayCoroutine = StartCoroutine(ShowWithDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (delayCoroutine != null) StopCoroutine(delayCoroutine);
        Invoke("CheckHide", 0.1f);
    }

    private void CheckHide() => ItemTooltip.Instance.HideTooltip();

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(1.0f);
        ItemTooltip.Instance.ShowTooltip(item, false, transform.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnBuyButtonClicked();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemTooltip.Instance.ShowTooltip(item, false, transform.position);
        }
    }

    public void OnBuyButtonClicked()
    {
        if (item != null && InventorySystem.Instance != null)
        {
            bool success = InventorySystem.Instance.BuyItem(item);
            if (success)
            {
                Merchant merchant = FindObjectOfType<Merchant>();
                if (merchant != null)
                {
                    merchant.shopItems.Clear();
                }

                Destroy(gameObject);
            }
        }
    }
}