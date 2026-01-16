using UnityEngine;

// Цей скрипт висить на обʼєкті тригер зони
// Він показує кнопку відкриття відео коли гравець стоїть у зоні
// Він ховає кнопку коли гравець виходить із зони
// Він повідомляє VideoLessonPlayer чи гравець зараз у зоні
public class ShowVideoButtonInTrigger : MonoBehaviour
{
    // Тут ми вручну вказуємо обʼєкт гравця
    // Це зроблено щоб тригер працював гарантовано і не залежав від тегів
    [SerializeField] private GameObject playerObject;

    // Це обʼєкт кнопки відкриття відео
    [SerializeField] private GameObject videoButtonObject;

    // Це скрипт який керує відео та UI
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    private void Start()
    {
        // На старті гри кнопку ховаємо
        HideButton();

        // На старті вважаємо що гравця в зоні немає
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Цей метод викликається коли якийсь обʼєкт зайшов у тригер

        // Перевіряємо що це саме гравець
        if (other.gameObject != playerObject) return;

        // Запамʼятовуємо що гравець у зоні
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(true);

        // Якщо відео вже відкрите, кнопку відкриття не показуємо
        if (videoLessonPlayer != null && videoLessonPlayer.IsVideoOpen())
            return;

        // Показуємо кнопку відкриття
        ShowButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Цей метод викликається коли якийсь обʼєкт вийшов із тригера

        // Перевіряємо що це саме гравець
        if (other.gameObject != playerObject) return;

        // Запамʼятовуємо що гравець більше не у зоні
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);

        // Ховаємо кнопку відкриття
        HideButton();

        // Якщо гравець пішов із зони, закриваємо відео
        if (videoLessonPlayer != null)
            videoLessonPlayer.CloseVideo();
    }

    private void ShowButton()
    {
        // Вмикаємо обʼєкт кнопки
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(true);
    }

    private void HideButton()
    {
        // Вимикаємо обʼєкт кнопки
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(false);
    }
}
