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
        Vector2 rayOrigin = transform.position;
        Vector2 rayDirection = pickupDirection;

        // Визуализация луча для отладки
        Debug.DrawRay(rayOrigin, rayDirection * pickupDistance, Color.green, 0.5f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, pickupDistance);

        if (hit.collider != null)
        {
            Debug.Log($"Hit: {hit.collider.name}");

            if (hit.collider.TryGetComponent(out WeightObject obj))
            {
                Debug.Log($"Picking up: {obj.name}");
                currentObject = obj;
                obj.transform.SetParent(holdPoint);
                obj.transform.localPosition = Vector3.zero;

                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.simulated = false;
            }
            else
            {
                Debug.Log("Object doesn't have WeightObject component");
            }
        }
        else
        {
            Debug.Log("Raycast didn't hit anything");
        }
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