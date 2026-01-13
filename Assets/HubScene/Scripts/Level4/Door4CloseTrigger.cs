using UnityEngine;

// Цей скрипт висить на тригері закривання дверей
// Коли гравець заходить у зону, двері закриваються
// Скрипт отримує двері як один обʼєкт і сам знаходить DoorSimple
// Скрипт отримує гравця як один обʼєкт, щоб не залежати від тегів
public class Door4CloseTrigger : MonoBehaviour
{
    // Обʼєкт дверей, які потрібно закрити
    [SerializeField] private GameObject doorObject;

    // Обʼєкт гравця
    // Сюди перетягни головний обʼєкт гравця з Hierarchy
    [SerializeField] private GameObject playerObject;

    // Спрайт цього тригера закривання
    // Після спрацювання його можна сховати
    [SerializeField] private SpriteRenderer mySprite;

    // Обʼєкт дороги або будь-який інший обʼєкт, який потрібно вимкнути
    [SerializeField] private GameObject road;

    // Компонент, який керує дверима
    private DoorSimple door;

    private void Awake()
    {
        // На старті сцени шукаємо DoorSimple на дверях
        door = FindDoorSimple(doorObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Якщо гравець не підключений, нічого не робимо
        if (playerObject == null)
            return;

        // Перевіряємо, що в зону зайшов саме наш гравець
        // Колайдер може бути на самому гравці або на його дочірньому обʼєкті
        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        // Якщо компонент не знайшовся у Awake, пробуємо знайти ще раз
        if (door == null)
            door = FindDoorSimple(doorObject);

        // Закриваємо двері
        if (door != null)
            door.Close();

        // Вимикаємо дорогу, якщо вона підключена
        if (road != null)
            road.SetActive(false);

        // Ховаємо спрайт цього тригера, якщо він підключений
        if (mySprite != null)
            mySprite.enabled = false;
    }

    // Метод для пошуку DoorSimple на дверях
    // Підходить для різної структури обʼєкта дверей
    private DoorSimple FindDoorSimple(GameObject obj)
    {
        if (obj == null)
            return null;

        // Шукаємо на самому обʼєкті
        DoorSimple controller = obj.GetComponent<DoorSimple>();
        if (controller != null)
            return controller;

        // Шукаємо в дочірніх обʼєктах
        controller = obj.GetComponentInChildren<DoorSimple>(true);
        if (controller != null)
            return controller;

        // Шукаємо в батьківському обʼєкті
        controller = obj.GetComponentInParent<DoorSimple>(true);
        return controller;
    }
}
