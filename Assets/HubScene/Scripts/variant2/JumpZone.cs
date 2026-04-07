using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpZone : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpHeight = 0.4f;
    public float jumpDuration = 0.35f;

    private void OnTriggerStay2D(Collider2D other)
    {
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

