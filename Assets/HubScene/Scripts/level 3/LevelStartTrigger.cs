using UnityEngine;

public class LevelStartTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что вошел именно игрок
        if (other.CompareTag("Player"))
        {
            EquationManager manager = FindObjectOfType<EquationManager>();
            if (manager != null)
            {
                manager.StartLevelTimer();
                gameObject.SetActive(false);
            }
        }
    }
}