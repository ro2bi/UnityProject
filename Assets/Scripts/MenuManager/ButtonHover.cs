using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("hovered", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("hovered", false);
    }
}
