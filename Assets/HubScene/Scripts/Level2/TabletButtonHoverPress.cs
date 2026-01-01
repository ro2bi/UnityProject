using UnityEngine;

public class TabletButtonHoverPress : MonoBehaviour
{
    public enum ButtonType
    {
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        Check
    }

    // Тип кнопки визначає яку дію вона виконує
    [SerializeField] private ButtonType type;

    // Посилання на планшет з логікою
    [SerializeField] private TabletWorldSimple tablet;

    // Спрайти для різних станів кнопки
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    private SpriteRenderer sr;
    private bool mouseOver = false;

    private void Awake()
    {
        // Отримуємо SpriteRenderer кнопки
        sr = GetComponent<SpriteRenderer>();

        // Встановлюємо стандартний вигляд кнопки
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    private void OnMouseEnter()
    {
        // Миша навелась на кнопку
        mouseOver = true;

        // Показуємо виділений стан
        if (sr != null && hoverSprite != null)
            sr.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        // Миша пішла з кнопки
        mouseOver = false;

        // Повертаємо стандартний вигляд
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    private void OnMouseDown()
    {
        // Кнопку натиснули і тримають
        if (sr != null && pressedSprite != null)
            sr.sprite = pressedSprite;
    }

    private void OnMouseUp()
    {
        // Кнопку відпустили
        // Повертаємо відповідний вигляд
        if (sr != null)
        {
            if (mouseOver && hoverSprite != null)
                sr.sprite = hoverSprite;
            else if (normalSprite != null)
                sr.sprite = normalSprite;
        }

        // Виконуємо дію тільки якщо миша була над кнопкою
        if (!mouseOver) return;
        if (tablet == null) return;

        if (type == ButtonType.XPlus) tablet.PressXPlus();
        if (type == ButtonType.XMinus) tablet.PressXMinus();
        if (type == ButtonType.YPlus) tablet.PressYPlus();
        if (type == ButtonType.YMinus) tablet.PressYMinus();
        if (type == ButtonType.Check) tablet.PressCheck();
    }
}
