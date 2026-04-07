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

    [SerializeField] private ButtonType type;

    [SerializeField] private TabletWorldSimple tablet;

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    private SpriteRenderer sr;
    private bool mouseOver = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    private void OnMouseEnter()
    {
        mouseOver = true;

        if (sr != null && hoverSprite != null)
            sr.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        mouseOver = false;

        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    private void OnMouseDown()
    {
        if (sr != null && pressedSprite != null)
            sr.sprite = pressedSprite;
    }

    private void OnMouseUp()
    {

        if (sr != null)
        {
            if (mouseOver && hoverSprite != null)
                sr.sprite = hoverSprite;
            else if (normalSprite != null)
                sr.sprite = normalSprite;
        }

        if (!mouseOver) return;
        if (tablet == null) return;

        if (!tablet.IsOpen) return;

        if (type == ButtonType.XPlus) tablet.PressXPlus();
        if (type == ButtonType.XMinus) tablet.PressXMinus();
        if (type == ButtonType.YPlus) tablet.PressYPlus();
        if (type == ButtonType.YMinus) tablet.PressYMinus();
        if (type == ButtonType.Check) tablet.PressCheck();
    }
}