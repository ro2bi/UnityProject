using UnityEngine;

public class EquationSlot : MonoBehaviour
{
    public bool isOccupied = false;
    public int currentValue;
    private WeightObject currentItem;

    [Header("Visual")]
    public SpriteRenderer slotRenderer;
    public Color occupiedColor = Color.yellow;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private void Awake() => slotRenderer = GetComponent<SpriteRenderer>();

    public void InsertItem(WeightObject item)
    {
        currentItem = item;
        currentValue = item.numericValue;
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
        currentValue = 0;
        isOccupied = false; // Слот теперь официально свободен!

        item.transform.SetParent(null);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb) rb.isKinematic = false;

        if (slotRenderer) slotRenderer.color = Color.white;

        return item;
    }

    public void ResetSlotManually()
    {
        if (currentItem != null)
        {
            Collider2D itemCollider = currentItem.GetComponent<Collider2D>();
            if (itemCollider) itemCollider.enabled = true;
        }

        currentItem = null;
        isOccupied = false;
        currentValue = 0;
        if (slotRenderer) slotRenderer.color = Color.white;
    }

    public void SetFeedback(bool isCorrect)
    {
        if (slotRenderer) slotRenderer.color = isCorrect ? correctColor : wrongColor;
    }

    public WeightObject GetItem() => currentItem;
}