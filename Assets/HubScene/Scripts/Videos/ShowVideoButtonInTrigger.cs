using UnityEngine;

// Цей скрипт висить на Trigger Zone
// Trigger має Collider2D з увімкненим IsTrigger
// Скрипт відповідає тільки за появу і зникнення кнопки
public class ShowVideoButtonInTrigger : MonoBehaviour
{
    // Обʼєкт гравця
    // Ми порівнюємо саме з ним
    [SerializeField] private GameObject playerObject;

    // Обʼєкт кнопки у світі
    [SerializeField] private GameObject videoButtonObject;

    // Головний скрипт відео
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    // Скрипт кнопки
    // Через нього ми скидаємо спрайт
    [SerializeField] private VideoButtonHoverPress videoButtonHoverPress;

    private void Start()
    {
        // На старті гри кнопку ховаємо
        HideButton();

        // Повідомляємо VideoLessonPlayer
        // Що гравця у зоні немає
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);

        // Також одразу скидаємо кнопку в normal
        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Цей метод викликається
        // Коли будь-який обʼєкт зайшов у тригер

        // Нас цікавить тільки гравець
        if (other.gameObject != playerObject) return;

        // Повідомляємо що гравець у зоні
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(true);

        // Якщо відео вже відкрите
        // Кнопку показувати не треба
        if (videoLessonPlayer != null && videoLessonPlayer.IsVideoOpen())
            return;

        // Перед показом кнопки
        // ОБОВʼЯЗКОВО скидаємо її у normal
        if (videoButtonHoverPress != null)
            videoButtonHoverPress.ResetToNormal();

        // Показуємо кнопку
        ShowButton();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Метод викликається
        // Коли обʼєкт виходить з тригера

        if (other.gameObject != playerObject) return;

        // Повідомляємо що гравець вийшов із зони
        if (videoLessonPlayer != null)
            videoLessonPlayer.SetPlayerInZone(false);

        // Ховаємо кнопку
        HideButton();

        // Якщо відео було відкрито
        // Ми його закриваємо
        if (videoLessonPlayer != null)
            videoLessonPlayer.CloseVideo();
    }

    private void ShowButton()
    {
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(true);
    }

    private void HideButton()
    {
        if (videoButtonObject == null) return;
        videoButtonObject.SetActive(false);
    }
}
