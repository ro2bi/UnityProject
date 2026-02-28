using UnityEngine;

public class LevelStartTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<EquationManager>()?.StartLevelTimer();
        }
    }
}