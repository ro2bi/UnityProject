using UnityEngine;

public class DoorLockTrigger2D : MonoBehaviour
{
    public DoorZoneLockController door;          // Сюди перетягни двері
    public bool disableAfterUse = true;          // Щоб тригер не спрацьовував повторно

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Коли гравець торкнувся тригеру - блокуємо двері назавжди
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.LockDoorForever();

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
