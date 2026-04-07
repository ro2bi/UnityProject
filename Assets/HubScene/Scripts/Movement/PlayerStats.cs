using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    private PlayerMovementNew movement;

    [Header("Base values")]
    private float baseSpeed;
    private float baseJumpHeight;
    private float baseJumpDuration;

    private Coroutine speedRoutine;
    private Coroutine jumpRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        movement = GetComponent<PlayerMovementNew>();

        baseSpeed = movement.speed;
        baseJumpHeight = movement.defaultJumpHeight;
        baseJumpDuration = movement.defaultJumpDuration;
    }

    public void ApplySpeedBoost(float bonus, float duration)
    {
        if (speedRoutine != null)
            StopCoroutine(speedRoutine);

        speedRoutine = StartCoroutine(SpeedBoostRoutine(bonus, duration));
    }

    private IEnumerator SpeedBoostRoutine(float bonus, float duration)
    {
        movement.speed = baseSpeed + bonus;
        yield return new WaitForSeconds(duration);
        movement.speed = baseSpeed;
    }

    public void ApplyJumpBoost(float heightBonus, float durationBonus, float duration)
    {
        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);

        jumpRoutine = StartCoroutine(JumpBoostRoutine(heightBonus, durationBonus, duration));
    }

    private IEnumerator JumpBoostRoutine(float heightBonus, float durationBonus, float duration)
    {
        movement.SetJumpParameters(
            baseJumpHeight + heightBonus,
            baseJumpDuration + durationBonus
        );

        yield return new WaitForSeconds(duration);

        movement.ResetJumpParameters();
    }
}
