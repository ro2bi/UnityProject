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

    public bool CanAccept(WeightObject item) => item != null && item.type == acceptedType;

    public void InsertItem(WeightObject item)
    {
        currentItem = item;
        isOccupied = true;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        item.SetVisibility(true);

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

        item.SetVisibility(false);

        Collider2D itemCollider = item.GetComponent<Collider2D>();
        if (itemCollider) itemCollider.enabled = true;

        currentItem = null;
        isOccupied = false;
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
            currentItem.SetVisibility(false);
            Collider2D itemCollider = currentItem.GetComponent<Collider2D>();
            if (itemCollider) itemCollider.enabled = true;
        }
        currentItem = null;
        isOccupied = false;
        if (slotRenderer) slotRenderer.color = Color.white;
    }

    public void SetFeedback(bool isCorrect)
    {
        if (slotRenderer) slotRenderer.color = isCorrect ? correctColor : wrongColor;
    }

    public string GetValueAsString()
    {
        if (currentItem == null) return "";
        return (currentItem.type == WeightObject.ItemType.Number)
            ? currentItem.numericValue.ToString()
            : currentItem.operatorSymbol;
    }

    public WeightObject GetItem() => currentItem;
}