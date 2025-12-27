using UnityEngine;

public class DoorZoneLockController : MonoBehaviour
{
    [Header("Колайдер дверей (блокує прохід)")]
    public Collider2D doorCollider;

    [Header("Візуал дверей (щоб зникала текстурка)")]
    public Renderer doorRenderer; // SpriteRenderer теж підходить, бо він Renderer

    [Header("Дорога/платформа, яка з'являється")]
    public GameObject roadObject;

    [Header("Стан")]
    public bool isLocked = false;   // Якщо true - двері назавжди заблоковані
    public bool isDoorOpen = false; // Поточний стан дверей (відкриті/закриті)

    private bool playerInsideZone = false;
    private bool roadPermanent = false;

    private void Start()
    {
        // На старті двері закриті
        CloseDoor();
        UpdateRoadState();
    }

    public void SetPlayerInsideZone(bool inside)
    {
        // Якщо двері заблоковані - зона більше не впливає
        if (isLocked) return;

        playerInsideZone = inside;

        if (playerInsideZone) OpenDoor();
        else CloseDoor();
    }

    public void LockDoorForever()
    {
        // Блокуємо двері назавжди, і робимо дорогу постійною
        isLocked = true;
        playerInsideZone = false;
        roadPermanent = true;

        // Двері мають бути закриті
        CloseDoor();
        UpdateRoadState();
    }

    public void OpenDoor()
    {
        if (isLocked) return;

        isDoorOpen = true;

        // Вимикаємо колайдер (прохід відкритий)
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Ховаємо візуал дверей (текстурку)
        if (doorRenderer != null)
            doorRenderer.enabled = false;

        UpdateRoadState();
    }

    public void CloseDoor()
    {
        isDoorOpen = false;

        // Вмикаємо колайдер (прохід закритий)
        if (doorCollider != null)
            doorCollider.enabled = true;

        // Показуємо візуал дверей назад
        // (якщо двері заблоковані, вони все одно мають бути "закриті" і видимі)
        if (doorRenderer != null)
            doorRenderer.enabled = true;

        UpdateRoadState();
    }

    private void UpdateRoadState()
    {
        if (roadObject == null) return;

        // Дорога активна якщо двері відкриті або стала постійною
        bool shouldBeActive = isDoorOpen || roadPermanent;
        roadObject.SetActive(shouldBeActive);
    }
}
