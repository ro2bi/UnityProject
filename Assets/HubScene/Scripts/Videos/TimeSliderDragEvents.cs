using UnityEngine;
using UnityEngine.EventSystems;

// Цей скрипт вішається на обʼєкт timeSlider
// Він ловить початок і кінець перетягування
// І повідомляє VideoLessonPlayer коли можна робити перемотку
public class TimeSliderDragEvents : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    // Посилання на VideoLessonPlayer
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Коли користувач почав тягнути слайдер
        // Повідомляємо VideoLessonPlayer щоб він дозволив перемотку
        if (videoLessonPlayer == null) return;
        videoLessonPlayer.BeginDragTimeSlider();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Коли користувач відпустив слайдер
        // Повідомляємо VideoLessonPlayer щоб він зробив фінальну перемотку
        if (videoLessonPlayer == null) return;
        videoLessonPlayer.EndDragTimeSlider();
    }
}
