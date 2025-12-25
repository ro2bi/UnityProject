using UnityEngine;

public class ArrowTriggerActivator : MonoBehaviour
{
    public TargetArrow2D arrow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            arrow.Show();
        }
    }
}
