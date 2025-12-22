using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 0.4f;      // Высота прыжка в этой зоне
    public float jumpDuration = 0.35f;   // Длительность прыжка в этой зоне

    private void OnTriggerStay2D(Collider2D other)
    {
        // 👇 ПОСТОЯННО ОБНОВЛЯЕМ, ПОКА ИГРОК В ЗОНЕ
        PlayerMovementNew player = other.GetComponent<PlayerMovementNew>();
        if (player != null)
        {
            player.SetJumpParameters(jumpHeight, jumpDuration);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovementNew player = other.GetComponent<PlayerMovementNew>();
        if (player != null)
        {
            player.ResetJumpParameters();
        }
    }
}

