using UnityEngine;

public class ScaleRevealTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        WeightObject item = other.GetComponent<WeightObject>();

        if (item != null)
        {
            item.SetVisibility(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        WeightObject item = other.GetComponent<WeightObject>();

        if (item != null)
        {
            item.SetVisibility(false);
        }
    }
}