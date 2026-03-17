using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Куда будем телепортировать:")]
    public Transform destination; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = destination.position;
        }
    }
}