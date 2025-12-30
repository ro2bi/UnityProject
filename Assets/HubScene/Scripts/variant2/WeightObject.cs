using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WeightObject : MonoBehaviour
{
    [Header("Physics")]
    public float weight = 1f;

    [Header("Logic")]
    public int numericValue; 

    public Vector3 startPosition; 

    public void ReturnToStart()
    {
        transform.SetParent(null); // Отцепляем от слота
        transform.position = startPosition; // Возвращаем на место
        GetComponent<Rigidbody2D>().isKinematic = false; // Включаем физику обратно
    }

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.mass = weight;

        startPosition = transform.position;
    }
}