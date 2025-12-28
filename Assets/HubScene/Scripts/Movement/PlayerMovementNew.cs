using UnityEngine;
using System.Collections;

public class PlayerMovementNew : MonoBehaviour
{
    // ... (Твои существующие переменные)
    [Header("Movement")]
    public float speed = 5f;

    [Header("Jump Visual")]
    public Transform visual;
    public float defaultJumpHeight = 0.4f;
    public float defaultJumpDuration = 0.35f;

    private float currentJumpHeight;
    private float currentJumpDuration;

    // 🔥 НОВЫЙ РАЗДЕЛ: ПОДБОР И ВЕС
    [Header("Pickup System")]
    public Transform holdPoint;       // Пустой объект перед игроком, куда "прилипнет" буква
    public float pickupRange = 1.2f;  // Радиус поиска предметов/слотов
    private WeightObject carriedItem; // Ссылка на то, что мы сейчас несем

    // ... (Остальные твои переменные: Ground Check, Fall, Checkpoint и т.д.)
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask islandLayer;

    [Header("Fall Settings")]
    public float maxFallTime = 3f;
    public float fallDepth = 1.5f;
    public float deathAnimTime = 1.2f;
    public float fallGravity = 2f;

    [Header("Checkpoint")]
    public AudioClip checkpointSound;

    [Header("Sorting")]
    public int normalSortingOrder = 1;
    public int fallingSortingOrder = -1;

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

        // ВЗАИМОДЕЙСТВИЕ (Кнопка E)
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            HandleInteraction();
        }

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

    // НОВАЯ ЛОГИКА: ВЗАИМОДЕЙСТВИЕ
    private void HandleInteraction()
    {
        if (carriedItem == null)
        {
            // Пытаемся подобрать
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange);
            foreach (var hit in hits)
            {
                WeightObject item = hit.GetComponent<WeightObject>();
                if (item != null)
                {
                    PickUp(item);
                    return;
                }
            }
        }
        else
        {
            // Пытаемся вставить в слот уравнения
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange);
            foreach (var hit in hits)
            {
                EquationSlot slot = hit.GetComponent<EquationSlot>();
                if (slot != null && !slot.isOccupied)
                {
                    PlaceIntoSlot(slot);
                    return;
                }
            }
            // Если слота нет рядом — просто бросаем
            Drop();
        }
    }

    private void PickUp(WeightObject item)
    {
        carriedItem = item;
        item.GetComponent<Rigidbody2D>().isKinematic = true;
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;

        // Утяжеляем персонажа: уменьшаем высоту прыжка на основе веса
        // (чем тяжелее вес, тем ниже прыжок)
        currentJumpHeight -= (item.weight * 0.05f);
    }

    

    private void Drop()
    {
        carriedItem.transform.SetParent(null);
        carriedItem.GetComponent<Rigidbody2D>().isKinematic = false;
        carriedItem = null;
        ResetJumpParameters();
    }

    // =========================
    // JUMP (Изменен для учета веса)
    // =========================
    IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim?.SetTrigger("Jump");

        // Если несем предмет, прыжок может стать короче (эффект g)
        float jumpTime = currentJumpDuration;
        float jumpH = currentJumpHeight;

        float half = jumpTime / 2f;
        float t = 0;

        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up *
                Mathf.Lerp(0, jumpH, t / half);
            yield return null;
        }

        t = 0;
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up *
                Mathf.Lerp(jumpH, 0, t / half);
            yield return null;
        }

        visual.localPosition = visualStartPos;
        isJumping = false;

        if (!IsGroundUnder())
            StartFall();
    }

    // ... (Остальные твои методы: FixedUpdate, StartFall, FallRoutine, DeathRoutine, EndFall, IsGroundUnder, OnTriggerEnter2D)
    void FixedUpdate()
    {
        if (isDead) return;
        Vector2 dir = isFalling ? (Vector2)fallVelocity : (isJumping ? jumpDirection : moveInput);
        if (isFalling) fallVelocity.y -= fallGravity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void StartFall()
    {
        if (isFalling) return;
        fallVelocity = lastMoveDirection.normalized;
        sprite.sortingOrder = fallingSortingOrder;
        StartCoroutine(FallRoutine());
    }

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
            if (IsGroundUnder()) { anim?.SetTrigger("Land"); EndFall(); yield break; }
            yield return null;
        }
        yield return StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true;
        anim?.SetTrigger("Death");
        yield return new WaitForSeconds(deathAnimTime);
        if (CheckpointData.HasCheckpoint) uiManager.ShowDeathScreen();
        else uiManager.GameOver();
    }

    void EndFall()
    {
        visual.localPosition = visualStartPos;
        sprite.sortingOrder = normalSortingOrder;
        isFalling = false;
        fallVelocity = Vector2.zero;
    }

    bool IsGroundUnder()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, islandLayer);
        foreach (var hit in hits) if (hit.gameObject != gameObject) return true;
        return false;
    }

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
        }
    }

    // =========================
    // JUMP ZONE SUPPORT (Твои методы)
    // =========================
    public void SetJumpParameters(float height, float duration)
    {
        currentJumpHeight = height;
        currentJumpDuration = duration;

        // Если при этом мы несем предмет, он ДОПОЛНИТЕЛЬНО занижает прыжок в этой зоне
        if (carriedItem != null)
            currentJumpHeight -= (carriedItem.weight * 0.05f);
    }

    public void ResetJumpParameters()
    {
        currentJumpHeight = defaultJumpHeight;
        currentJumpDuration = defaultJumpDuration;

        // Если бросили предмет, но мы в зоне, зона вернет свои параметры через OnTriggerStay
        if (carriedItem != null)
            currentJumpHeight -= (carriedItem.weight * 0.05f);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
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

    private void PlaceIntoSlot(EquationSlot slot)
    {
        // Мы передаем весь объект carriedItem в слот
        slot.InsertItem(carriedItem);

        carriedItem = null;
        ResetJumpParameters(); // Возвращаем прыжок в норму после того, как избавились от тяжести
    }
}