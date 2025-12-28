using UnityEngine;
using System.Linq; // Нужно для работы с массивами (метод Contains)

public class EquationSlot : MonoBehaviour
{
    [Header("Settings")]
    // Теперь это массив. В инспекторе нажми на "+", чтобы добавить несколько чисел
    public int[] correctValues;

    public bool isOccupied = false;
    public bool isCorrect = false;

    [Header("Visual Feedback")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    private SpriteRenderer slotRenderer;

    private void Awake()
    {
        slotRenderer = GetComponent<SpriteRenderer>();
    }

    public void InsertItem(WeightObject item)
    {
        isOccupied = true;

        // Физическое прикрепление
        item.transform.SetParent(this.transform);
        item.transform.localPosition = Vector3.zero;
        item.GetComponent<Rigidbody2D>().isKinematic = true;

        // ПРОВЕРКА: есть ли значение предмета в нашем списке правильных чисел?
        if (correctValues.Contains(item.numericValue))
        {
            isCorrect = true;
            if (slotRenderer) slotRenderer.color = correctColor;
            Debug.Log($"Слот: ВЕРНО! Число {item.numericValue} подходит.");
        }
        else
        {
            isCorrect = false;
            if (slotRenderer) slotRenderer.color = wrongColor;
            Debug.Log($"Слот: НЕВЕРНО! Число {item.numericValue} не в списке.");
        }

        // Оповещаем менеджер о том, что данные в слоте изменились
        FindObjectOfType<EquationManager>()?.CheckFullEquation();
    }

    public void RemoveItem()
    {
        isOccupied = false;
        isCorrect = false;
        if (slotRenderer) slotRenderer.color = Color.white;
    }
}