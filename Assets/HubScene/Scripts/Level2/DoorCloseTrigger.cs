using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    // Посилання на двері
    public DoorSimple door;

    // Спрайт цього тригера
    public SpriteRenderer mySprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, що торкнувся гравець
        if (!other.CompareTag("Player"))
            return;

        // Закриваємо двері
        door.Close();

        // Ховаємо текстуру цього тригера
        mySprite.enabled = false;
    }
}
