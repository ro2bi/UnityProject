using UnityEngine;

public class GravityModeButton : MonoBehaviour
{
    [Header("Settings")]
    public float targetHeight = 0.2f;
    public float targetDuration = 0.2f;
    public float moveSpeed = 3f;

    public float gravityG = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovementNew player = other.GetComponent<PlayerMovementNew>();

            if (player != null)
            {
                player.defaultJumpHeight = targetHeight;
                player.defaultJumpDuration = targetDuration;
                player.speed = moveSpeed;

                player.gravityMultiplier = gravityG;

                player.ResetJumpParameters();
            }
        }
    }
}