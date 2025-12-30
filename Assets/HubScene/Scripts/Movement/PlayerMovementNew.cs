using UnityEngine;
using System.Collections;

public class PlayerMovementNew : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Jump Visual")]
    public Transform visual;
    public float defaultJumpHeight = 0.4f;
    public float defaultJumpDuration = 0.35f;

    private float currentJumpHeight;
    private float currentJumpDuration;

    // 🔥 НОВОЕ: Множитель гравитации (G)
    [Header("Gravity (G) System")]
    public float gravityMultiplier = 1f; // 1 = норма, >1 = тяжелый, <1 = легкий

    [Header("Pickup System")]
    public Transform holdPoint;
    public float pickupRange = 1.2f;
    private WeightObject carriedItem;

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

    public void OnItemPlacedIntoSlot()
    {
        carriedItem = null;
        ResetJumpParameters();
    }

    void Update()
    {
        if (isDead) return;

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

    


    private void PickUp(WeightObject item)
    {
        carriedItem = item;
        item.GetComponent<Rigidbody2D>().isKinematic = true;
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
        currentJumpHeight -= (item.weight * 0.05f);
    }

    private void Drop()
    {
        if (carriedItem == null) return;

        // 1. Сохраняем текущую позицию в МИРОВЫХ координатах
        Vector3 worldPos = carriedItem.transform.position;

        // 2. Сначала отцепляем от родителя
        carriedItem.transform.SetParent(null);

        // 3. Сразу после этого принудительно ставим в сохраненную мировую позицию
        // Это предотвратит скачки из-за масштаба (Scale)
        carriedItem.transform.position = worldPos;

        // 4. Работа с физикой
        Rigidbody2D itemRb = carriedItem.GetComponent<Rigidbody2D>();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.gravityScale = 0; // Для топ-дауна строго 0
            itemRb.velocity = Vector2.zero; // Гасим инерцию
        }

        // 5. САМОЕ ВАЖНОЕ: Чтобы предмет не "взорвался" при столкновении с игроком,
        // немного отодвинем его в сторону, куда смотрел игрок.
        carriedItem.transform.position += (Vector3)lastMoveDirection * 0.5f;

        carriedItem = null;
        ResetJumpParameters();
    }

    // =========================
    // JUMP (Учитывает G)
    // =========================
    // =========================
    // JUMP (Измененный для влияния G на высоту и время)
    // =========================
    IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim?.SetTrigger("Jump");

        // Формула: чем меньше gravityMultiplier, тем больше высота и время
        float jumpTime = currentJumpDuration / gravityMultiplier;
        float jumpH = currentJumpHeight / gravityMultiplier;

        float half = jumpTime / 2f;
        float t = 0;

        // Взлет
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up * Mathf.Lerp(0, jumpH, t / half);
            yield return null;
        }

        t = 0;
        // Падение на землю
        while (t < half)
        {
            t += Time.deltaTime;
            visual.localPosition = visualStartPos + Vector3.up * Mathf.Lerp(jumpH, 0, t / half);
            yield return null;
        }

        visual.localPosition = visualStartPos;
        isJumping = false;

        if (!IsGroundUnder())
            StartFall();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Vector2 dir = isFalling ? (Vector2)fallVelocity : (isJumping ? jumpDirection : moveInput);

        //  При падении учитываем множитель гравитации для горизонтального сноса
        if (isFalling) fallVelocity.y -= (fallGravity * gravityMultiplier) * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    void StartFall()
    {
        if (isFalling) return;
        fallVelocity = lastMoveDirection.normalized;
        sprite.sortingOrder = fallingSortingOrder;
        StartCoroutine(FallRoutine());
    }

    // =========================
    // FALL (Учитывает G)
    // =========================
    IEnumerator FallRoutine()
    {
        isFalling = true;
        anim?.SetTrigger("Fall");

        float visualProgress = 0; // Для анимации движения вниз
        float deathTimer = 0;      // Для отсчета реального времени до смерти

        // Пока не вышло РЕАЛЬНОЕ время (maxFallTime)
        while (deathTimer < maxFallTime)
        {
            // 1. Считаем реальное время (не зависит от G)
            deathTimer += Time.deltaTime;

            // 2. Считаем прогресс падения (зависит от G)
            // При G=10 visualProgress станет равным 1 за 0.3 секунды
            visualProgress += (Time.deltaTime / maxFallTime) * gravityMultiplier;
            visualProgress = Mathf.Clamp01(visualProgress); // Ограничиваем от 0 до 1

            // 3. Двигаем спрайт вниз
            float y = Mathf.Lerp(0, -fallDepth, visualProgress);
            visual.localPosition = visualStartPos + Vector3.up * y;

            // 4. Проверяем, коснулись ли мы пола (дна ямы)
            if (IsGroundUnder())
            {
                anim?.SetTrigger("Land");
                EndFall();
                yield break;
            }

            yield return null;
        }

        // Если за 3 секунды реального времени мы так и не нашли землю — умираем
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

    public void SetJumpParameters(float height, float duration)
    {
        currentJumpHeight = height;
        currentJumpDuration = duration;

        if (carriedItem != null)
            currentJumpHeight -= (carriedItem.weight * 0.05f);
    }

    public void ResetJumpParameters()
    {
        currentJumpHeight = defaultJumpHeight;
        currentJumpDuration = defaultJumpDuration;
        //gravityMultiplier = 1f; // Сброс G в норму

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

    private void HandleInteraction()
    {
        float interactRange = pickupRange; // твоя дистанция подбора
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        if (carriedItem == null) // ПЫТАЕМСЯ ВЗЯТЬ
        {
            foreach (var hit in hits)
            {
                // 1. Проверяем, не слот ли это с предметом внутри
                EquationSlot slot = hit.GetComponent<EquationSlot>();
                if (slot != null && slot.isOccupied)
                {
                    WeightObject taken = slot.RemoveItem();
                    if (taken != null)
                    {
                        PickUp(taken);
                        return;
                    }
                }

                // 2. Если слота нет, ищем просто лежащий предмет
                WeightObject item = hit.GetComponent<WeightObject>();
                if (item != null)
                {
                    PickUp(item);
                    return;
                }
            }
        }
        else // ПЫТАЕМСЯ ПОЛОЖИТЬ
        {
            foreach (var hit in hits)
            {
                EquationSlot slot = hit.GetComponent<EquationSlot>();
                if (slot != null && !slot.isOccupied) // Если нашли свободный слот
                {
                    PlaceIntoSlot(slot);
                    return;
                }
            }

            Drop(); // Если рядом нет слота — просто бросаем
        }
    }

    private void PlaceIntoSlot(EquationSlot slot)
    {
        WeightObject itemToPlace = carriedItem;
        carriedItem = null; // Сначала освобождаем руки игрока!
        ResetJumpParameters(); // Сбрасываем физику прыжка

        slot.InsertItem(itemToPlace);
    }

}