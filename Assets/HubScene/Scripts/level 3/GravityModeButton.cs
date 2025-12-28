using UnityEngine;

public class GravityModeButton : MonoBehaviour
{
    [Header("Settings")]
    public float targetHeight = 0.2f;    // Насколько высоко прыгнет
    public float targetDuration = 0.2f;  // Как быстро упадет (чем меньше, тем резче g)
    public float moveSpeed = 3f;         // Можно менять и скорость ходьбы

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovementNew player = other.GetComponent<PlayerMovementNew>();

            // 1. Меняем базовые параметры в скрипте игрока навсегда (до следующей кнопки)
            player.defaultJumpHeight = targetHeight;
            player.defaultJumpDuration = targetDuration;
            player.speed = moveSpeed;

            // 2. Обновляем текущие параметры (с учетом веса предмета в руках)
            player.ResetJumpParameters();

        }
    }
}
