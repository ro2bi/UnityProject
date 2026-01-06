using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
                              IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image itemIcon;
    public GameObject emptySlotIndicator;
    [HideInInspector] public int slotIndex;

    private ItemData currentItem;
    private Coroutine delayCoroutine;
    private GameObject dragIcon; // Временная иконка при перетаскивании
    private Canvas canvas;

    public bool IsEmpty => currentItem == null;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void EquipCurrentItem()
    {
        if (currentItem != null)
        {
            InventorySystem.Instance.EquipItem(currentItem);
            ItemTooltip.Instance.HideTooltip(); // Скрываем после нажатия
        }
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;
        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            if (emptySlotIndicator) emptySlotIndicator.SetActive(false);
        }
        else { ClearSlot(); }
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        if (emptySlotIndicator) emptySlotIndicator.SetActive(true);
    }

    // --- ЛОГИКА ПЕРЕТАСКИВАНИЯ (DRAG & DROP) ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // Создаем визуальную копию иконки для перетаскивания
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform);
        dragIcon.transform.SetAsLastSibling();

        Image img = dragIcon.AddComponent<Image>();
        img.sprite = currentItem.icon;
        img.raycastTarget = false; // Чтобы иконка не мешала определять, над чем мышка
        dragIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);

        itemIcon.color = new Color(1, 1, 1, 0.5f); // Делаем иконку в слоте прозрачной
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);
        itemIcon.color = Color.white;

        // ПРОВЕРКА: Вынесли ли мы предмет за пределы UI?
        // Если мышка НЕ над объектом UI, значит мы в мире
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            InventorySystem.Instance.DropItem(currentItem);
            ItemTooltip.Instance.HideTooltip();
        }
    }

    // --- ОСТАЛЬНАЯ ЛОГИКА (ТУЛТИПЫ И КЛИКИ) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
            delayCoroutine = StartCoroutine(ShowWithDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (delayCoroutine != null) StopCoroutine(delayCoroutine);

        // Даем небольшую задержку перед скрытием, чтобы успеть перевести мышку
        Invoke("CheckHide", 0.1f);
    }

    private void CheckHide() => ItemTooltip.Instance.HideTooltip();

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(1.0f);
        ItemTooltip.Instance.ShowTooltip(currentItem, true, transform.position);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemTooltip.Instance.ShowTooltip(currentItem, true, transform.position);
        }
    }
}