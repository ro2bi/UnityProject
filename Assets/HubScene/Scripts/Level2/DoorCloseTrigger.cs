using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public DoorSimple door;

    public SpriteRenderer mySprite;

    public GameObject road;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (door != null)
        {
            door.Close();
        }

        if (road != null)
        {
            road.SetActive(false);
        }

        if (mySprite != null)
        {
            mySprite.enabled = false;
        }
    }
}