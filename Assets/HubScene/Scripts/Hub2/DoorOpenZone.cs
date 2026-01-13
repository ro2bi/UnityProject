using UnityEngine;

// Цей скрипт висить на обʼєкті з тригером біля дверей
// Коли гравець заходить у зону, двері відкриваються
// Коли гравець виходить із зони, двері закриваються
public class DoorOpenZone : MonoBehaviour
{
    // Обʼєкт дверей, якими буде керувати ця зона
    // Сюди потрібно перетягнути двері з Hierarchy
    [SerializeField] private GameObject doorObject;

    // Обʼєкт гравця
    // Сюди потрібно перетягнути головний обʼєкт гравця
    [SerializeField] private GameObject playerObject;

    // Якщо false, зона не впливає на двері
    // Це зручно, коли інший скрипт бере керування на себе
    [SerializeField] private bool zoneActive = true;

    // Посилання на компонент, який безпосередньо відкриває і закриває двері
    private DoorZoneLockController door;

    private void Awake()
    {
        // Під час старту сцени шукаємо компонент керування дверима
        // Він може бути на самих дверях, у їхніх дітях або у батьківському обʼєкті
        door = FindDoorController(doorObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Якщо зона вимкнена, нічого не робимо
        if (!zoneActive)
            return;

        // Перевіряємо, чи це саме гравець зайшов у зону
        // Колайдер може бути або на самому гравці, або на його дочірньому обʼєкті
        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        // Якщо контролер дверей знайдений, повідомляємо що гравець у зоні
        if (door != null)
            door.SetPlayerInsideZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Якщо зона вимкнена, нічого не робимо
        if (!zoneActive)
            return;

        // Перевіряємо, чи це саме гравець вийшов із зони
        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        // Повідомляємо дверям що гравець більше не в зоні
        if (door != null)
            door.SetPlayerInsideZone(false);
    }

    // Цей метод може викликатися з іншого скрипта
    // Після виклику зона повністю перестає керувати дверима
    public void DisableZone()
    {
        // Вимикаємо активність зони
        zoneActive = false;

        // На випадок, якщо гравець був у зоні в момент вимкнення
        // Примусово закриваємо двері
        if (door != null)
            door.SetPlayerInsideZone(false);
    }

    // Метод для пошуку компонента керування дверима
    // Дозволяє підключати будь який обʼєкт дверей без точної структури
    private DoorZoneLockController FindDoorController(GameObject obj)
    {
        // Якщо двері не підключені, нічого не шукаємо
        if (obj == null)
            return null;

        // Шукаємо компонент прямо на обʼєкті дверей
        DoorZoneLockController controller = obj.GetComponent<DoorZoneLockController>();
        if (controller != null)
            return controller;

        // Шукаємо компонент у дочірніх обʼєктах
        controller = obj.GetComponentInChildren<DoorZoneLockController>(true);
        if (controller != null)
            return controller;

        // Шукаємо компонент у батьківському обʼєкті
        controller = obj.GetComponentInParent<DoorZoneLockController>(true);
        return controller;
    }
}
