using UnityEngine;

public class ScaleRevealTrigger : MonoBehaviour
{
    // Срабатывает, когда черный квадрат входит в зону весов
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ищем скрипт WeightObject на предмете, который вошел в зону
        WeightObject item = other.GetComponent<WeightObject>();

        if (item != null)
        {
            // Проявляем число
            item.SetVisibility(true);
        }
    }

    // Срабатывает, когда черный квадрат покидает зону весов
    private void OnTriggerExit2D(Collider2D other)
    {
        WeightObject item = other.GetComponent<WeightObject>();

        if (item != null)
        {
            // Скрываем число обратно
            item.SetVisibility(false);
        }
    }
}