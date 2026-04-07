using UnityEngine;

public class DoorOpenZone : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;

    [SerializeField] private GameObject playerObject;

    [SerializeField] private bool zoneActive = true;

    private DoorZoneLockController door;

    private void Awake()
    {
        door = FindDoorController(doorObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!zoneActive)
            return;

        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        if (door != null)
            door.SetPlayerInsideZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!zoneActive)
            return;

        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        if (door != null)
            door.SetPlayerInsideZone(false);
    }

    public void DisableZone()
    {
        zoneActive = false;

        if (door != null)
            door.SetPlayerInsideZone(false);
    }

    private DoorZoneLockController FindDoorController(GameObject obj)
    {
        if (obj == null)
            return null;

        DoorZoneLockController controller = obj.GetComponent<DoorZoneLockController>();
        if (controller != null)
            return controller;

        controller = obj.GetComponentInChildren<DoorZoneLockController>(true);
        if (controller != null)
            return controller;

        controller = obj.GetComponentInParent<DoorZoneLockController>(true);
        return controller;
    }
}