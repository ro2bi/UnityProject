using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    // Усі колайдери дверей які знаходяться у дочірніх обєктах
    // Це можуть бути дві частини або більше
    private Collider2D[] doorColliders;

    // Усі спрайти дверей у дочірніх обєктах
    // Вони відповідають за візуальне відображення дверей
    private SpriteRenderer[] doorSprites;

    private void Awake()
    {
        // Знаходимо всі колайдери у дочірніх обєктах
        // Це дозволяє не задавати їх вручну
        doorColliders = GetComponentsInChildren<Collider2D>();

        // Знаходимо всі спрайти у дочірніх обєктах
        // Таким чином ми керуємо всіма частинами дверей одразу
        doorSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        // Цей метод викликається коли гравець правильно вирішив головоломку

        // Вимикаємо всі колайдери щоб гравець міг пройти
        foreach (Collider2D col in doorColliders)
        {
            col.enabled = false;
        }

        // Ховаємо всі спрайти щоб двері зникли візуально
        foreach (SpriteRenderer sr in doorSprites)
        {
            sr.enabled = false;
        }
    }

    public void CloseDoor()
    {
        // Цей метод використовується на старті рівня
        // Він повертає двері у закритий стан

        // Вмикаємо всі колайдери щоб заблокувати прохід
        foreach (Collider2D col in doorColliders)
        {
            col.enabled = true;
        }

        // Показуємо всі спрайти дверей
        foreach (SpriteRenderer sr in doorSprites)
        {
            sr.enabled = true;
        }
    }
}
