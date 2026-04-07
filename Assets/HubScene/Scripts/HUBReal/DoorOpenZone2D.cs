using UnityEngine;

public class DoorOpenZone2D : MonoBehaviour
{
    public DoorZoneLockController door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.SetPlayerInsideZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.SetPlayerInsideZone(false);
    }
}
