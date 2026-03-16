using UnityEngine;

public class UmovaVideoButton : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    [SerializeField] private GameObject imagePanel;
    [SerializeField] private GameObject darkBackground;
    [SerializeField] private GameObject closeButton;

    private SpriteRenderer sr;
    private bool mouseOver;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        ResetToNormal();
    }

    public void ResetToNormal()
    {
        mouseOver = false;

        if (sr == null) return;
        if (normalSprite == null) return;

        sr.sprite = normalSprite;
    }

    private void OnMouseEnter()
    {
        mouseOver = true;

        if (hoverSprite != null)
            sr.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        ResetToNormal();
    }

    private void OnMouseDown()
    {
        if (pressedSprite != null)
            sr.sprite = pressedSprite;
    }

    private void OnMouseUp()
    {
        if (mouseOver && hoverSprite != null)
            sr.sprite = hoverSprite;
        else
            ResetToNormal();

        if (!mouseOver) return;

        if (imagePanel != null) imagePanel.SetActive(true);
        if (darkBackground != null) darkBackground.SetActive(true);
        if (closeButton != null) closeButton.SetActive(true);

        gameObject.SetActive(false);
    }
}