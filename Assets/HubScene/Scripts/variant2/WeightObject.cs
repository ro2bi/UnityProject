using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class WeightObject : MonoBehaviour
{
    public float weight; // масса предмета

    private void Start()
    {
        // Автоматически устанавливаем массу в Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = weight;
        }
    }
}