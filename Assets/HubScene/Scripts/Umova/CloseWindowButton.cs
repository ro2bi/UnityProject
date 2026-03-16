using UnityEngine;

public class CloseWindowButton : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    [SerializeField] private GameObject imagePanel;
    [SerializeField] private GameObject darkBackground;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject openButton;

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

    private void ResetToNormal()
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

        if (imagePanel != null) imagePanel.SetActive(false);
        if (darkBackground != null) darkBackground.SetActive(false);
        if (closeButton != null) closeButton.SetActive(false);

        if (openButton != null) openButton.SetActive(true);
    }
}