using UnityEngine;

public class Door4CloseTrigger : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;

    [SerializeField] private GameObject playerObject;

    [SerializeField] private SpriteRenderer mySprite;

    [SerializeField] private GameObject road;

    private DoorSimple door;

    private void Awake()
    {
        door = FindDoorSimple(doorObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerObject == null)
            return;

        if (other.gameObject != playerObject &&
            other.transform.root.gameObject != playerObject)
            return;

        if (door == null)
            door = FindDoorSimple(doorObject);

        if (door != null)
            door.Close();

        if (road != null)
            road.SetActive(false);

        if (mySprite != null)
            mySprite.enabled = false;
    }

    private DoorSimple FindDoorSimple(GameObject obj)
    {
        if (obj == null)
            return null;

        DoorSimple controller = obj.GetComponent<DoorSimple>();
        if (controller != null)
            return controller;

        controller = obj.GetComponentInChildren<DoorSimple>(true);
        if (controller != null)
            return controller;

        controller = obj.GetComponentInParent<DoorSimple>(true);
        return controller;
    }
}