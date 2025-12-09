using UnityEngine;
using UnityEngine.UI;

public class Scale : MonoBehaviour
{
    [Header("Scale Settings")]
    public Text displayText; // UI Text для отображения веса
    public Transform scalePlatform; // Платформа весов

    private float currentWeight = 0f;
    private PickableItem itemOnScale;

    void Update()
    {
        // Обновляем отображение
        if (displayText != null)
        {
            displayText.text = $"Вес: {currentWeight:F1} Н";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item != null && !item.isHeld)
        {
            itemOnScale = item;
            CalculateWeight();
        }
    }

    void OnTriggerExit(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item == itemOnScale)
        {
            itemOnScale = null;
            currentWeight = 0f;
        }
    }

    public void CalculateWeight()
    {
        if (itemOnScale != null)
        {
            float gravity = GravityController.Instance.GetCurrentGravity();
            currentWeight = itemOnScale.GetMass() * gravity;
        }
        else
        {
            currentWeight = 0f;
        }
    }

    public float GetCurrentWeight()
    {
        return currentWeight;
    }
}