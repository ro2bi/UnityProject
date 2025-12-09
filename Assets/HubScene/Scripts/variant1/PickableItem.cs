using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("Item Properties")]
    public float mass = 50f; // Масса предмета
    public string itemName = "Item";

    [HideInInspector]
    public bool isHeld = false;

    private Rigidbody rb;
    private Collider col;
    private Transform originalParent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        originalParent = transform.parent;

        rb.mass = mass;
    }

    public void Pickup(Transform holdPosition)
    {
        isHeld = true;

        // Отключаем физику
        rb.isKinematic = true;
        col.enabled = false;

        // Присоединяем к игроку
        transform.SetParent(holdPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        isHeld = false;

        // Включаем физику
        rb.isKinematic = false;
        col.enabled = true;

        // Отсоединяем от игрока
        transform.SetParent(originalParent);
    }

    public float GetMass()
    {
        return mass;
    }
}