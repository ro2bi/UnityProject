using UnityEngine;

// Цей скрипт висить БЕЗПОСЕРЕДНЬО на кнопці у світі
// Кнопка є спрайтовою і має Collider2D
// Скрипт відповідає тільки за зовнішній вигляд кнопки
// Він не знає нічого про тригери
// Він не знає нічого про UI
// Його задача міняти спрайти і відкривати відео
public class VideoButtonHoverPress : MonoBehaviour
{
    [Header("Спрайти для станів кнопки")]

    // Спрайт який видно коли кнопка просто стоїть
    [SerializeField] private Sprite normalSprite;

    // Спрайт який видно коли миша наведена на кнопку
    [SerializeField] private Sprite hoverSprite;

    // Спрайт який видно коли кнопку натиснули
    [SerializeField] private Sprite pressedSprite;

    [Header("Скрипт який керує відео")]

    // Посилання на VideoLessonPlayer
    // Через нього ми відкриваємо відео
    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    // SpriteRenderer кнопки
    // Через нього ми міняємо зображення
    private SpriteRenderer sr;

    // Прапорець який показує чи миша зараз над кнопкою
    private bool mouseOver;

    private void Awake()
    {
        // Беремо SpriteRenderer з цього ж обʼєкта
        // Без нього ми не зможемо міняти спрайти
        sr = GetComponent<SpriteRenderer>();

        // На старті гри кнопка завжди повинна бути
        // у початковому вигляді
        ResetToNormal();
    }

    public void ResetToNormal()
    {
        // ЦЕ ДУЖЕ ВАЖЛИВИЙ МЕТОД
        // Його викликають ІНШІ скрипти
        // Коли кнопка зʼявляється
        // Або коли відео закривається

        // Ми скидаємо стан миші
        mouseOver = false;

        // Якщо щось не підключено, просто виходимо
        if (sr == null) return;
        if (normalSprite == null) return;

        // Ставимо початковий спрайт
        sr.sprite = normalSprite;
    }

    private void OnMouseEnter()
    {
        // Unity автоматично викликає цей метод
        // Коли курсор миші заходить на Collider кнопки

        mouseOver = true;

        if (sr == null) return;
        if (hoverSprite == null) return;

        // Ставимо спрайт наведення
        sr.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        // Unity викликає цей метод
        // Коли курсор виходить з Collider кнопки

        mouseOver = false;

        // Повертаємо кнопку у початковий стан
        ResetToNormal();
    }

    private void OnMouseDown()
    {
        // Unity викликає цей метод
        // Коли кнопку миші НАТИСНУЛИ

        if (sr == null) return;
        if (pressedSprite == null) return;

        // Ставимо спрайт натискання
        sr.sprite = pressedSprite;
    }

    private void OnMouseUp()
    {
        // Unity викликає цей метод
        // Коли кнопку миші ВІДПУСТИЛИ

        if (sr == null) return;

        // Якщо миша все ще над кнопкою
        // Повертаємо hover
        if (mouseOver && hoverSprite != null)
        {
            sr.sprite = hoverSprite;
        }
        else
        {
            // Якщо ні, повертаємо normal
            ResetToNormal();
        }

        // Відкривати відео можна тільки
        // Якщо кнопку відпустили над нею
        if (!mouseOver) return;

        // Якщо VideoLessonPlayer не підключений, нічого не робимо
        if (videoLessonPlayer == null) return;

        // Викликаємо відкриття відео
        videoLessonPlayer.OpenVideo();
    }
}
