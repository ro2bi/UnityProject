using UnityEngine;

public class DoorLockTrigger2D : MonoBehaviour
{
    public DoorZoneLockController door;
    public bool disableAfterUse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (door != null)
            door.LockDoorForever();

        if (disableAfterUse)
            gameObject.SetActive(false);
    }
}
