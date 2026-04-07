using UnityEngine;

public class VideoButtonHoverPress : MonoBehaviour
{
    [Header("Спрайти для станів кнопки")]

    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Скрипт який керує відео")]

    [SerializeField] private VideoLessonPlayer videoLessonPlayer;

    private SpriteRenderer sr;
    private bool mouseOver;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ResetToNormal();
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

        if (sr == null) return;
        if (hoverSprite == null) return;

        sr.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        ResetToNormal();
    }

    private void OnMouseDown()
    {
        if (sr == null) return;
        if (pressedSprite == null) return;

        sr.sprite = pressedSprite;
    }

    private void OnMouseUp()
    {
        if (sr == null) return;

        if (mouseOver && hoverSprite != null)
        {
            sr.sprite = hoverSprite;
        }
        else
        {
            ResetToNormal();
        }

        if (!mouseOver) return;
        if (videoLessonPlayer == null) return;

        videoLessonPlayer.OpenVideo();
    }
}