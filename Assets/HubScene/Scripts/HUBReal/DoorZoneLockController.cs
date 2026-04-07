using UnityEngine;

public class DoorZoneLockController : MonoBehaviour
{
    [Header("Колайдер дверей (блокує прохід)")]
    public Collider2D doorCollider;

    [Header("Візуал дверей (щоб зникала текстурка)")]
    public Renderer doorRenderer;

    [Header("Дорога/платформа, яка з'являється")]
    public GameObject roadObject;

    [Header("Стан")]
    public bool isLocked = false;
    public bool isDoorOpen = false;

    private bool playerInsideZone = false;
    private bool roadPermanent = false;

    private void Start()
    {
        CloseDoor();
        UpdateRoadState();
    }

    public void SetPlayerInsideZone(bool inside)
    {
        if (isLocked) return;

        playerInsideZone = inside;

        if (playerInsideZone) OpenDoor();
        else CloseDoor();
    }

    public void LockDoorForever()
    {
        isLocked = true;
        playerInsideZone = false;
        roadPermanent = true;

        CloseDoor();
        UpdateRoadState();
    }

    public void OpenDoor()
    {
        if (isLocked) return;

        isDoorOpen = true;

        if (doorCollider != null)
            doorCollider.enabled = false;

        if (doorRenderer != null)
            doorRenderer.enabled = false;

        UpdateRoadState();
    }

    public void CloseDoor()
    {
        isDoorOpen = false;

        if (doorCollider != null)
            doorCollider.enabled = true;

        if (doorRenderer != null)
            doorRenderer.enabled = true;

        UpdateRoadState();
    }

    private void UpdateRoadState()
    {
        if (roadObject == null) return;

        bool shouldBeActive = isDoorOpen || roadPermanent;
        roadObject.SetActive(shouldBeActive);
    }
}