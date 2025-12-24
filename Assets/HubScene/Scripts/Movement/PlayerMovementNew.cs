using UnityEngine;
using System.Collections;

public class PlayerMovementNew : MonoBehaviour
{
    // =========================
    // MOVEMENT
    // =========================
    [Header("Movement")]
    public float speed = 5f;

    // =========================
    // VISUAL JUMP (TOP-DOWN)
    // =========================
    [Header("Jump Visual")]
    public Transform visual;
    public float defaultJumpHeight = 0.4f;
    public float defaultJumpDuration = 0.35f;

    private float currentJumpHeight;
    private float currentJumpDuration;

    // =========================
    // GROUND CHECK
    // =========================
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask islandLayer;

    // =========================
    // FALL / DEATH
    // =========================
    [Header("Fall Settings")]
    public float maxFallTime = 3f;
    public float fallDepth = 1.5f;
    public float deathAnimTime = 1.2f;
    public float fallGravity = 2f;

    // =========================
    // CHECKPOINT
    // =========================
    [Header("Checkpoint")]
    public AudioClip checkpointSound;

    // =========================
    // SORTING
    // =========================
    [Header("Sorting")]
    public int normalSortingOrder = 1;
    public int fallingSortingOrder = -1;

    // =========================
    // PRIVATE
    // =========================
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private UIManagerNew uiManager;

    private Vector2 moveInput;
    private Vector2 jumpDirection;
    private Vector2 lastMoveDirection;
    private Vector2 fallVelocity;

    private bool isJumping;
    private bool isFalling;
    private bool isDead;

    private Vector3 visualStartPos;
    private Vector3 lastSafePosition;

    // =========================
    // START
    // =========================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = visual.GetComponent<SpriteRenderer>();
        uiManager = FindObjectOfType<UIManagerNew>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        visualStartPos = visual.localPosition;
        sprite.sortingOrder = normalSortingOrder;

        currentJumpHeight = defaultJumpHeight;
        currentJumpDuration = defaultJumpDuration;

        KeybindManager.InitializeKeys();

        // 🔥 RESPAWN ПОСЛЕ ПЕРЕЗАГРУЗКИ СЦЕНЫ
        if (CheckpointData.HasCheckpoint)
        {
            transform.position = CheckpointData.LastCheckpointPosition;
            lastSafePosition = transform.position;
        }
        else
        {
            lastSafePosition = transform.position;
        }
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        if (isDead || isFalling) return;

        if (!isJumping && !IsGroundUnder())
        {
            StartFall();
            return;
        }

        float h = 0;
        float v = 0;

        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_RIGHT))) h = 1;
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_LEFT))) h = -1;

        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_FORWARD))) v = 1;
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_BACKWARD))) v = -1;

        moveInput = new Vector2(h, v).normalized;

        if (moveInput != Vector2.zero)
            lastMoveDirection = moveInput;

        if (anim != null)
        {
            anim.SetBool("IsMovingForward", v > 0);
            anim.SetBool("IsMovingBackward", v < 0);
            anim.SetBool("IsMovingRight", h > 0);
            anim.SetBool("IsMovingLeft", h < 0);
        }

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.JUMP)))
        {
            if (!isJumping)
            {
                lastSafePosition = transform.position;
                jumpDirection = moveInput;
                StartCoroutine(JumpRoutine());
            }
        }
    }

    // =========================
    // FIXED UPDATE
    // =========================
    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 dir;

        if (isFalling)
        {
            fallVelocity.y -= fallGravity * Time.fixedDeltaTime;
            dir = fallVelocity;
        }
        else
        {
            dir = isJumping ? jumpDirection : moveInput;
        }

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    // =========================
    // JUMP
    // =========================
    IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim?.SetTrigger("Jump");

        float half = currentJumpDuration / 2f;
        float t = 0;

        // 👆 ВЗЛЁТ
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up *
                Mathf.Lerp(0, currentJumpHeight, t / half);
            yield return null;
        }

        // 👇 ПРИЗЕМЛЕНИЕ
        t = 0;
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up *
                Mathf.Lerp(currentJumpHeight, 0, t / half);
            yield return null;
        }

        visual.localPosition = visualStartPos;
        isJumping = false;

        if (!IsGroundUnder())
            StartFall();
    }

    // =========================
    // START FALL
    // =========================
    void StartFall()
    {
        if (isFalling) return;

        fallVelocity = lastMoveDirection.normalized;
        sprite.sortingOrder = fallingSortingOrder;
        StartCoroutine(FallRoutine());
    }

    // =========================
    // FALL
    // =========================
    IEnumerator FallRoutine()
    {
        isFalling = true;
        anim?.SetTrigger("Fall");

        float timer = 0;

        while (timer < maxFallTime)
        {
            timer += Time.deltaTime;

            float y = Mathf.Lerp(0, -fallDepth, timer / maxFallTime);
            visual.localPosition = visualStartPos + Vector3.up * y;

            if (IsGroundUnder())
            {
                anim?.SetTrigger("Land");
                EndFall();
                yield break;
            }

            yield return null;
        }

        yield return StartCoroutine(DeathRoutine());
    }

    // =========================
    // DEATH
    // =========================
    IEnumerator DeathRoutine()
    {
        isDead = true;
        anim?.SetTrigger("Death");

        yield return new WaitForSeconds(deathAnimTime);

        if (CheckpointData.HasCheckpoint)
            uiManager.ShowDeathScreen();
        else
            uiManager.GameOver();
    }

    // =========================
    // END FALL
    // =========================
    void EndFall()
    {
        visual.localPosition = visualStartPos;
        sprite.sortingOrder = normalSortingOrder;
        isFalling = false;
        fallVelocity = Vector2.zero;
    }

    // =========================
    // GROUND CHECK
    // =========================
    bool IsGroundUnder()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            groundCheck.position,
            groundCheckRadius,
            islandLayer
        );

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
                return true;
        }
        return false;
    }

    // =========================
    // CHECKPOINT TRIGGER
    // =========================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("checkpoint"))
        {
            CheckpointData.LastCheckpointPosition = collision.transform.position;
            CheckpointData.HasCheckpoint = true;

            lastSafePosition = collision.transform.position;

            if (checkpointSound != null && SoundManager.instance != null)
                SoundManager.instance.PlaySound(checkpointSound);

            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>()?.SetTrigger("appear");

            Debug.Log("🟢 CHECKPOINT SAVED");
        }
    }

    // =========================
    // JUMP ZONE SUPPORT
    // =========================
    public void SetJumpParameters(float height, float duration)
    {
        currentJumpHeight = height;
        currentJumpDuration = duration;
    }

    public void ResetJumpParameters()
    {
        currentJumpHeight = defaultJumpHeight;
        currentJumpDuration = defaultJumpDuration;
    }

    // =========================
    // GIZMOS
    // =========================
    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void ApplyJumpBuff(float heightBonus, float durationBonus, float time)
    {
        StopAllCoroutines();
        StartCoroutine(JumpBuffRoutine(heightBonus, durationBonus, time));
    }

    IEnumerator JumpBuffRoutine(float h, float d, float time)
    {
        currentJumpHeight = defaultJumpHeight + h;
        currentJumpDuration = defaultJumpDuration + d;

        yield return new WaitForSeconds(time);

        ResetJumpParameters();
    }
}