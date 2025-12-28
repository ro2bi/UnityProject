using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WeightObject : MonoBehaviour
{
    [Header("Physics")]
    public float weight = 1f; // Влияет на прыжок

    [Header("Logic")]
    public int numericValue;  // Само число (например, 2, 5, 9)

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.mass = weight;
    }
}