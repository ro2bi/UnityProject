using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Â ענטדדונ גמר¸כ: " + other.name);
    }
}
