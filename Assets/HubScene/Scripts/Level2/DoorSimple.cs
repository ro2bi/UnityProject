using UnityEngine;

public class DoorSimple : MonoBehaviour
{
    // Усі колайдери дверей у дочірніх обʼєктах
    private Collider2D[] doorColliders;

    // Усі спрайти дверей у дочірніх обʼєктах
    private SpriteRenderer[] doorSprites;

    private void Awake()
    {
        // Знаходимо всі колайдери у дочірніх обʼєктах
        doorColliders = GetComponentsInChildren<Collider2D>();

        // Знаходимо всі спрайти у дочірніх обʼєктах
        doorSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Відкрити двері
    public void Open()
    {
        // Вимикаємо всі колайдери дверей
        foreach (Collider2D col in doorColliders)
            col.enabled = false;

        // Ховаємо всі спрайти дверей
        foreach (SpriteRenderer sr in doorSprites)
            sr.enabled = false;
    }

    // Закрити двері
    public void Close()
    {
        // Вмикаємо всі колайдери дверей
        foreach (Collider2D col in doorColliders)
            col.enabled = true;

        // Показуємо всі спрайти дверей
        foreach (SpriteRenderer sr in doorSprites)
            sr.enabled = true;
    }
}
