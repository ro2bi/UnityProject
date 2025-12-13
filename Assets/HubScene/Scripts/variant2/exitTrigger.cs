using UnityEngine;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("Стіна яка активується")]
    public GameObject closingWall;

    private void OnTriggerExit2D(Collider2D other)
    {
        // Коли гравець ВИХОДИТЬ з зони - активуємо стіну
        if (other.CompareTag("Player"))
        {
            if (closingWall != null)
            {
                closingWall.SetActive(true);
            }
        }
    }
}