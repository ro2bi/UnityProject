using UnityEngine;

public class ActivateWallOnlyTrigger : MonoBehaviour
{
    [SerializeField] private GameObject wallParent;

    [SerializeField] private bool hideTriggerSprite = true;

    [SerializeField] private bool disableTriggerCollider = true;

    private Collider2D cachedTriggerCollider;
    private SpriteRenderer cachedTriggerSprite;

    private void Awake()
    {
        cachedTriggerCollider = GetComponent<Collider2D>();
        cachedTriggerSprite = GetComponent<SpriteRenderer>();

        if (cachedTriggerCollider == null)
        {
            Debug.LogWarning("На обʼєкті тригера немає Collider2D, OnTriggerEnter2D не буде викликатись", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (wallParent == null)
        {
            Debug.LogWarning("Не підключено wallParent у інспекторі, немає що активувати", this);
            MakeTriggerOneTime();
            return;
        }

        wallParent.SetActive(true);

        EnableChildrenRenderersAndColliders(wallParent);

        MakeTriggerOneTime();
    }

    private void EnableChildrenRenderersAndColliders(GameObject parent)
    {
        var renderers = parent.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].enabled = true;
        }

        var colliders2D = parent.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < colliders2D.Length; i++)
        {
            if (colliders2D[i] != null)
                colliders2D[i].enabled = true;
        }
    }

    private void MakeTriggerOneTime()
    {
        if (disableTriggerCollider && cachedTriggerCollider != null)
        {
            cachedTriggerCollider.enabled = false;
        }

        if (hideTriggerSprite && cachedTriggerSprite != null)
        {
            cachedTriggerSprite.enabled = false;
        }
    }
}