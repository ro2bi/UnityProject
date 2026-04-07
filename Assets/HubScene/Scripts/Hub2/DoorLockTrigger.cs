using UnityEngine;

public class DoorLockTrigger : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;

    [SerializeField] private bool disableAfterUse = true;

    private DoorZoneLockController door;

    private void Awake()
    {
        door = FindDoorController(doorObject);

        if (door == null && doorObject != null)
        {
            Debug.LogWarning("DoorLockTrigger не знайшов DoorZoneLockController на doorObject, у дітях або у батьках", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (door == null)
            door = FindDoorController(doorObject);

        if (door != null)
            door.LockDoorForever();

        if (disableAfterUse)
            gameObject.SetActive(false);
    }

    private DoorZoneLockController FindDoorController(GameObject obj)
    {
        if (obj == null)
            return null;

        var controller = obj.GetComponent<DoorZoneLockController>();
        if (controller != null)
            return controller;

        controller = obj.GetComponentInChildren<DoorZoneLockController>(true);
        if (controller != null)
            return controller;

        controller = obj.GetComponentInParent<DoorZoneLockController>(true);
        return controller;
    }
}