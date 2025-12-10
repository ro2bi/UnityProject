using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupDistance = 1f;
    public Vector2 pickupDirection = Vector2.right; // Настраиваемое направление
    private WeightObject currentObject;

    void Update()
    {
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            if (currentObject == null)
                TryPickUp();
            else
                Drop();
        }
    }

    void TryPickUp()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Старт луча немного впереди игрока
        Vector2 rayOrigin = (Vector2)transform.position + direction * 0.6f;

        Debug.DrawRay(rayOrigin, direction * pickupDistance, Color.green, 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, pickupDistance, LayerMask.GetMask("Default"));

        if (!hit.collider)
        {
            Debug.Log("Raycast didn't hit anything");
            return;
        }

        Debug.Log($"Hit: {hit.collider.name}");

        WeightObject obj =
            hit.collider.GetComponent<WeightObject>() ??
            hit.collider.GetComponentInParent<WeightObject>() ??
            hit.collider.GetComponentInChildren<WeightObject>();

        if (obj == null)
        {
            Debug.Log("Object doesn't have WeightObject component");
            return;
        }

        currentObject = obj;

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        Debug.Log("Picked up!");
    }


    void Drop()
    {
        if (currentObject == null) return;

        currentObject.transform.SetParent(null);
        Rigidbody2D rb = currentObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = true;
        currentObject = null;
    }
}