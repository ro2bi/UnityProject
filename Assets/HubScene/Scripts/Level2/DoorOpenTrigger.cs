using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    public DoorSimple door;

    public SpriteRenderer mySprite;

    public SpriteRenderer closeTriggerSprite;

    private void Start()
    {
        mySprite.enabled = true;

        closeTriggerSprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        door.Open();

        mySprite.enabled = false;

        closeTriggerSprite.enabled = true;
    }
}