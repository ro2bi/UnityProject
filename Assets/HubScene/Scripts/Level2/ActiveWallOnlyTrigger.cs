using UnityEngine;

// Цей скрипт висить на тригері
// При дотику гравця:
// 1) вмикає БАТЬКІВСЬКИЙ обʼєкт стіни (там можуть бути 2+ частини)
// 2) вимикає сам тригер
public class ActivateWallOnlyTrigger : MonoBehaviour
{
    // Перетягни сюди БАТЬКІВСЬКИЙ обʼєкт стіни з Hierarchy
    [SerializeField] private GameObject wallParent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, що це саме гравець
        if (!other.CompareTag("Player"))
            return;

        // Вмикаємо стіну (разом з усіма дочірніми обʼєктами)
        if (wallParent != null)
            wallParent.SetActive(true);

        // Вимикаємо тригер після використання
        gameObject.SetActive(false);
    }
}
