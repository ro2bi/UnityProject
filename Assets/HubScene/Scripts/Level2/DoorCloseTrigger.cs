using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    // Посилання на скрипт дверей, які потрібно закрити
    public DoorSimple door;

    // Спрайт цього тригера, щоб його можна було сховати після активації
    public SpriteRenderer mySprite;

    // Посилання на дорогу або будь-який інший обʼєкт, який потрібно вимкнути
    // Цей обʼєкт буде повністю деактивований
    public GameObject road;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, чи в тригер зайшов саме гравець
        if (!other.CompareTag("Player"))
            return;

        // Закриваємо двері через метод скрипта DoorSimple
        if (door != null)
        {
            door.Close();
        }

        // Вимикаємо дорогу, якщо вона підключена в інспекторі
        // GameObject повністю зникає зі сцени і перестає працювати
        if (road != null)
        {
            road.SetActive(false);
        }

        // Ховаємо спрайт цього тригера, щоб він більше не був видимий
        if (mySprite != null)
        {
            mySprite.enabled = false;
        }
    }
}
