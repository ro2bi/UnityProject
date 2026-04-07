using UnityEngine;

public class DoorPuzzle : MonoBehaviour
{
    private Collider2D[] doorColliders;

    private SpriteRenderer[] doorSprites;

    private void Awake()
    {
        doorColliders = GetComponentsInChildren<Collider2D>();

        doorSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        foreach (Collider2D col in doorColliders)
        {
            col.enabled = false;
        }

        foreach (SpriteRenderer sr in doorSprites)
        {
            sr.enabled = false;
        }
    }

    public void CloseDoor()
    {
        foreach (Collider2D col in doorColliders)
        {
            col.enabled = true;
        }

        foreach (SpriteRenderer sr in doorSprites)
        {
            sr.enabled = true;
        }
    }
}