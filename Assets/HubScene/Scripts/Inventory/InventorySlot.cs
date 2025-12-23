using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
                              IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI компоненты")]
    public Image itemIcon;
    public GameObject emptySlotIndicator;

    [HideInInspector]
    public int slotIndex;

    private ItemData currentItem;
    private Canvas canvas;
    private GameObject draggedObject;
    private Vector3 originalPosition;
    private Transform originalParent;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (currentItem != null)
        {
            itemIcon.sprite = currentItem.icon;
            itemIcon.enabled = true;
            if (emptySlotIndicator != null)
                emptySlotIndicator.SetActive(false);
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        if (emptySlotIndicator != null)
            emptySlotIndicator.SetActive(true);
    }

    // Начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // Создаем временный объект для визуального перетаскивания
        draggedObject = new GameObject("DraggedItem");
        draggedObject.transform.SetParent(canvas.transform);
        draggedObject.transform.SetAsLastSibling();

        Image image = draggedObject.AddComponent<Image>();
        image.sprite = currentItem.icon;
        image.raycastTarget = false;

        RectTransform rt = draggedObject.GetComponent<RectTransform>();
        rt.sizeDelta = itemIcon.rectTransform.sizeDelta;

        originalPosition = transform.position;
        originalParent = transform.parent;

        // Делаем оригинальную иконку полупрозрачной
        Color color = itemIcon.color;
        color.a = 0.5f;
        itemIcon.color = color;
    }

    // Во время перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            draggedObject.transform.position = eventData.position;
        }
    }

    // Конец перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedObject != null)
        {
            Destroy(draggedObject);
        }

        // Восстанавливаем прозрачность
        Color color = itemIcon.color;
        color.a = 1f;
        itemIcon.color = color;
    }

    // Когда предмет сбрасывают на этот слот
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot != this)
        {
            // Меняем местами предметы
            InventorySystem.Instance.MoveItem(draggedSlot.slotIndex, slotIndex);
        }
    }

    // Наведение мыши
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            ItemTooltip.Instance.ShowTooltip(currentItem, true);
        }
    }

    // Уход мыши
    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    // Клик мыши (ПКМ и двойной клик)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // Правый клик
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
            return;
        }

        // Двойной клик левой кнопкой
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickThreshold)
            {
                UseItem();
                lastClickTime = 0f; // Сбрасываем, чтобы не было тройного клика
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    private void UseItem()
    {
        InventorySystem.Instance.UseItem(currentItem);
        ItemTooltip.Instance.HideTooltip();
    }

    // Методы для тултипа (вызываются из ItemTooltip)
    public void OnBuyButtonClicked()
    {
        // В инвентаре кнопка "купить" не имеет смысла
        // Но можем оставить для единообразия или удалить
    }

    public void OnSellButtonClicked()
    {
        if (currentItem != null)
        {
            InventorySystem.Instance.SellItem(currentItem);
            ItemTooltip.Instance.HideTooltip();
        }
    }
}