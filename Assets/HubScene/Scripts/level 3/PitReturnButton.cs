using UnityEngine;

public class PitTeleport : MonoBehaviour
{
    [Header("Точка возврата")]
    public Transform returnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Что-то вошло: " + other.name);

        if (other.CompareTag("Player"))
        {

            if (returnPoint == null)
            {
                Debug.LogError("Return Point НЕ назначена!");
                return;
            }

            Debug.Log("Игрок вошёл в триггер. Телепортируем.");

            other.transform.position = returnPoint.position;
        }
    }
}
