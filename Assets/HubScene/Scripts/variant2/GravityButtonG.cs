using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GravityButtonG : MonoBehaviour
{
    public bool increase;                     // true = G+, false = GЦ
    public GravitySliderFrames slider;        // ссылка на слайдер
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private SpriteRenderer sr;
    private bool playerInside = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = normalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            sr.sprite = normalSprite;
        }
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
            Press();
    }

    void Press()
    {
        sr.sprite = pressedSprite;

        if (increase)
            slider.IncreaseG();
        else
            slider.DecreaseG();
    }
}
