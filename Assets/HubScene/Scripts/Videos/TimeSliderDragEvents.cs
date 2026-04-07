using UnityEngine;
using UnityEngine.EventSystems;

public class TimeSliderDragEvents : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (videoLessonPlayer == null) return;
        videoLessonPlayer.BeginDragTimeSlider();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (videoLessonPlayer == null) return;
        videoLessonPlayer.EndDragTimeSlider();
    }
}
