using UnityEngine;

// Цей скрипт стоїть на спрайтовій кнопці у світі
// Він міняє спрайт при наведенні миші
// Він міняє спрайт при натисканні
// Він викликає відкриття відео через VideoLessonPlayer
public class VideoButtonHoverPress : MonoBehaviour
{
    [Header("Спрайти для станів кнопки")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Скрипт який керує відео та UI")]
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    private SpriteRenderer sr;
    private bool mouseOver;

    private void Awake()
    {
        // Беремо SpriteRenderer щоб міняти зображення кнопки
        sr = GetComponent<SpriteRenderer>();

        // На старті ставимо звичайний спрайт
        SetNormal();
    }

    private void OnMouseEnter()
    {
        // Курсор зайшов на колайдер кнопки
        mouseOver = true;
        SetHover();
    }

    private void OnMouseExit()
    {
        // Курсор вийшов з колайдера кнопки
        mouseOver = false;
        SetNormal();
    }

    private void OnMouseDown()
    {
        // Натиснули кнопку миші по кнопці
        SetPressed();
    }

    private void OnMouseUp()
    {
        // Відпустили кнопку миші

        // Повертаємо вигляд кнопки
        if (mouseOver) SetHover();
        else SetNormal();

        // Дію виконуємо тільки якщо відпустили над кнопкою
        if (!mouseOver) return;

        // Якщо не підключили VideoLessonPlayer, виходимо
        if (videoLessonPlayer == null) return;

        // Відкриваємо відео
        videoLessonPlayer.OpenVideo();
    }

    private void SetNormal()
    {
        if (sr == null) return;
        if (normalSprite == null) return;
        sr.sprite = normalSprite;
    }

    private void SetHover()
    {
        if (sr == null) return;
        if (hoverSprite == null) return;
        sr.sprite = hoverSprite;
    }

    private void SetPressed()
    {
        if (sr == null) return;
        if (pressedSprite == null) return;
        sr.sprite = pressedSprite;
    }
}
