using UnityEngine;

public class EquationSlot : MonoBehaviour
{
    [Header("Slot Configuration")]
    public WeightObject.ItemType acceptedType;

    public bool isOccupied = false;
    private WeightObject currentItem;

    [Header("Visual")]
    public SpriteRenderer slotRenderer;
    public Color occupiedColor = Color.yellow;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private void Awake() => slotRenderer = GetComponent<SpriteRenderer>();

    public bool CanAccept(WeightObject item)
    {
        // Проверяем: совпадает ли тип предмета с типом, который ждет слот
        return item != null && item.type == acceptedType;
    }

    public void InsertItem(WeightObject item)
    {
        currentItem = item;
        isOccupied = true;

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        // Отключаем коллайдер у предмета, чтобы он не мешал нажимать на слот
        Collider2D itemCollider = item.GetComponent<Collider2D>();
        if (itemCollider) itemCollider.enabled = false;

        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb) rb.isKinematic = true;

        if (slotRenderer) slotRenderer.color = occupiedColor;

        FindObjectOfType<EquationManager>()?.CheckFullEquation();
    }

    public WeightObject RemoveItem()
    {
        if (currentItem == null) return null;

        WeightObject item = currentItem;

        // ВКЛЮЧАЕМ коллайдер предмета обратно, чтобы его можно было нести
        Collider2D itemCollider = item.GetComponent<Collider2D>();
        if (itemCollider) itemCollider.enabled = true;

        // ПОЛНЫЙ СБРОС СОСТОЯНИЯ
        currentItem = null;
        isOccupied = false; // Слот теперь официально свободен!

        item.transform.SetParent(null);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb) rb.isKinematic = false;

        if (slotRenderer) slotRenderer.color = Color.white;

        return item;
    }

    // В файле EquationSlot.cs
    public void ResetSlotManually()
    {
        if (currentItem != null)
        {
            // Включаем коллайдер предмету, чтобы игрок снова мог его подобрать
            Collider2D itemCollider = currentItem.GetComponent<Collider2D>();
            if (itemCollider) itemCollider.enabled = true;
        }

        currentItem = null;   // Стираем ссылку на предмет
        isOccupied = false;   // Клетка теперь официально свободна!
                              // currentValue = 0;  // Если используешь старую систему с int

        // Возвращаем клетке обычный цвет (белый)
        if (slotRenderer) slotRenderer.color = Color.white;
    }

    public void SetFeedback(bool isCorrect)
    {
        if (slotRenderer) slotRenderer.color = isCorrect ? correctColor : wrongColor;
    }

    

    public string GetValueAsString()
    {
        if (currentItem == null) return "";
        if (currentItem.type == WeightObject.ItemType.Number)
            return currentItem.numericValue.ToString();
        else
            return currentItem.operatorSymbol;
    }

    public WeightObject GetItem() => currentItem;
}