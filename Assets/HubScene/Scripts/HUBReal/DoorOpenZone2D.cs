using UnityEngine;

public class DoorOpenZone2D : MonoBehaviour
{
    public DoorZoneLockController door; // Сюди перетягни об'єкт дверей з DoorZoneLockController

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Спрацьовує, коли гравець заходить у зону
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.SetPlayerInsideZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Спрацьовує, коли гравець виходить із зони
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.SetPlayerInsideZone(false);
    }
}
