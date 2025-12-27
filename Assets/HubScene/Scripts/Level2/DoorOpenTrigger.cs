using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    // Посилання на двері
    public DoorSimple door;

    // Спрайт цього тригера
    public SpriteRenderer mySprite;

    // Спрайт другого тригера
    public SpriteRenderer closeTriggerSprite;

    private void Start()
    {
        // На старті цей тригер видно
        mySprite.enabled = true;

        // На старті тригер закриття невидимий
        closeTriggerSprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, що торкнувся гравець
        if (!other.CompareTag("Player"))
            return;

        // Відкриваємо двері
        door.Open();

        // Ховаємо текстуру цього тригера
        mySprite.enabled = false;

        // Показуємо текстуру тригера закриття
        closeTriggerSprite.enabled = true;
    }
}
