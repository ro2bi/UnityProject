using UnityEngine;

// Цей скрипт висить на тригері відкриття дверей
// Коли гравець заходить у зону, двері відкриваються
// Скрипт отримує двері як один обʼєкт і сам знаходить DoorSimple
// Скрипт отримує гравця як один обʼєкт, щоб не залежати від тегів
public class Door4OpenTrigger : MonoBehaviour
{
    // Обʼєкт дверей, які потрібно відкрити
    [SerializeField] private GameObject doorObject;

    // Обʼєкт гравця
    // Сюди перетягни головний обʼєкт гравця з Hierarchy
    [SerializeField] private GameObject playerObject;

    // Спрайт цього тригера відкриття
    [SerializeField] private SpriteRenderer mySprite;

    // Спрайт тригера закриття
    [SerializeField] private SpriteRenderer closeTriggerSprite;

    // Компонент, який керує дверима
    private DoorSimple door;

    private void Awake()
    {
        // На старті шукаємо DoorSimple на дверях
        door = FindDoorSimple(doorObject);
    }

    private void Start()
    {
        // На старті цей тригер видно
        if (mySprite != null)
            mySprite.enabled = true;

        // На старті тригер закриття невидимий
        if (closeTriggerSprite != null)
            closeTriggerSprite.enabled = false;
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

        // Якщо з якоїсь причини компонент не знайшовся у Awake,
        // пробуємо знайти його ще раз під час входу
        if (door == null)
            door = FindDoorSimple(doorObject);

        // Відкриваємо двері
        if (door != null)
            door.Open();

        // Ховаємо спрайт цього тригера
        if (mySprite != null)
            mySprite.enabled = false;

        // Показуємо спрайт тригера закриття
        if (closeTriggerSprite != null)
            closeTriggerSprite.enabled = true;
    }

    // Метод для пошуку DoorSimple на дверях
    // Підходить для різної структури обʼєкта дверей
    private DoorSimple FindDoorSimple(GameObject obj)
    {
        if (obj == null)
            return null;

        // Спочатку шукаємо на самому обʼєкті
        DoorSimple controller = obj.GetComponent<DoorSimple>();
        if (controller != null)
            return controller;

        // Потім шукаємо в дочірніх обʼєктах
        controller = obj.GetComponentInChildren<DoorSimple>(true);
        if (controller != null)
            return controller;

        // Потім шукаємо в батьківському обʼєкті
        controller = obj.GetComponentInParent<DoorSimple>(true);
        return controller;
    }
}
