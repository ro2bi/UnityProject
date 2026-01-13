using UnityEngine;

// Цей скрипт висить на 2D тригері
// При дотику гравця двері блокуються назавжди
// В інспекторі підключається обʼєкт дверей як GameObject, а не як компонент
public class DoorLockTrigger : MonoBehaviour
{
    // Сюди перетягни обʼєкт дверей з Hierarchy
    // На ньому має бути компонент DoorZoneLockController, або він має бути в дітях або в батьках
    [SerializeField] private GameObject doorObject;

    // Чи потрібно вимикати тригер після використання
    // Це робить його одноразовим
    [SerializeField] private bool disableAfterUse = true;

    // Знайдений компонент керування дверима
    private DoorZoneLockController door;

    private void Awake()
    {
        // Намагаємось автоматично знайти компонент DoorZoneLockController
        door = FindDoorController(doorObject);

        // Якщо компонент не знайдено, виводимо попередження
        if (door == null && doorObject != null)
        {
            Debug.LogWarning("DoorLockTrigger не знайшов DoorZoneLockController на doorObject, у дітях або у батьках", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, що торкнувся саме гравець
        if (!other.CompareTag("Player"))
            return;

        // Якщо компонент дверей не знайдений, пробуємо знайти ще раз
        if (door == null)
            door = FindDoorController(doorObject);

        // Блокуємо двері назавжди
        if (door != null)
            door.LockDoorForever();

        // Вимикаємо тригер після спрацювання
        if (disableAfterUse)
            gameObject.SetActive(false);
    }

    private DoorZoneLockController FindDoorController(GameObject obj)
    {
        // Якщо обʼєкт не підключено, повертати нічого
        if (obj == null)
            return null;

        // Спочатку шукаємо компонент на самому обʼєкті
        var controller = obj.GetComponent<DoorZoneLockController>();
        if (controller != null)
            return controller;

        // Потім шукаємо в дочірніх обʼєктах
        controller = obj.GetComponentInChildren<DoorZoneLockController>(true);
        if (controller != null)
            return controller;

        // Потім шукаємо в батьківських обʼєктах
        controller = obj.GetComponentInParent<DoorZoneLockController>(true);
        return controller;
    }
}
