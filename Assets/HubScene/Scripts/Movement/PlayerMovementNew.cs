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

    [Header("Gravity (G) System")]
    public float gravityMultiplier = 1f;

    [Header("Pickup System (Items)")]
    public Transform holdPoint;
    public float holdPointDistance = 0.6f; // Дистанция для предметов
    public float pickupRange = 0.4f;
    private WeightObject carriedItem;

    [Header("Mine System (Only Mines)")]
    public Transform minePointer;         // Новый объект-указатель
    public float minePointerDistance = 1.1f; // Дистанция указателя (обычно дальше рук)
    public GameObject minePointerVisual;  // Ссылка на спрайт указателя (чтобы скрывать/показывать)

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
    private Vector2 lastMoveDirection = Vector2.down;
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

        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

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

    void Update()
    {
        if (MineGridManager2D.IsUIOpen)
        {
            rb.velocity = Vector2.zero;
            if (anim != null)
            {
                anim.SetBool("IsMovingForward", false);
                anim.SetBool("IsMovingBackward", false);
                anim.SetBool("IsMovingRight", false);
                anim.SetBool("IsMovingLeft", false);
            }
            return;
        }

        if (isDead) return;

        // Обновляем положение обеих точек (рук и указателя мин)
        UpdatePointers();

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            HandleInteraction();
        }

        if (!isJumping && !isFalling && !IsGroundUnder())
        {
            StartFall();
            return;
        }

        float h = 0; float v = 0;
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_RIGHT))) h = 1;
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_LEFT))) h = -1;
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_FORWARD))) v = 1;
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_BACKWARD))) v = -1;

        moveInput = new Vector2(h, v).normalized;
        if (moveInput != Vector2.zero) lastMoveDirection = moveInput;

        if (anim != null)
        {
            anim.SetBool("IsMovingForward", v > 0);
            anim.SetBool("IsMovingBackward", v < 0);
            anim.SetBool("IsMovingRight", h > 0);
            anim.SetBool("IsMovingLeft", h < 0);
            anim.SetFloat("LookX", lastMoveDirection.x);
            anim.SetFloat("LookY", lastMoveDirection.y);
        }

        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.JUMP)))
        {
            if (!isJumping && !isFalling)
            {
                lastSafePosition = transform.position;
                jumpDirection = moveInput;
                StartCoroutine(JumpRoutine());
            }
        }
    }

    private void UpdatePointers()
    {
        // 1. Позиция HoldPoint (Предметы)
        if (holdPoint != null)
        {
            holdPoint.localPosition = (Vector3)lastMoveDirection * holdPointDistance;

            // Сортировка переносимого предмета
            if (carriedItem != null)
            {
                SpriteRenderer itemSR = carriedItem.GetComponent<SpriteRenderer>();
                if (itemSR != null)
                    itemSR.sortingOrder = (lastMoveDirection.y > 0.1f) ? normalSortingOrder - 1 : normalSortingOrder + 1;
            }
        }

        // 2. Позиция MinePointer (Указатель для мин)
        if (minePointer != null)
        {
            minePointer.localPosition = (Vector3)lastMoveDirection * minePointerDistance;
        }
    }

    private void HandleInteraction()
    {
        Collider2D interactHit = Physics2D.OverlapCircle(minePointer.position, 0.3f);
        if (interactHit != null)
        {
            IInteractable interactable = interactHit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
                return; // Взаимодействовали — выходим
            }
        }

        // 2. ЛОГИКА МИН (если не нашли торговца)
        Collider2D cellHit = Physics2D.OverlapPoint(minePointer.position);
        if (cellHit != null)
        {
            MineCell2D cell = cellHit.GetComponent<MineCell2D>();
            if (cell != null)
            {
                cell.ToggleFlag();
                return;
            }
        }

        // --- ЛОГИКА ПРЕДМЕТОВ (Работает через HoldPoint) ---
        Collider2D[] itemHits = Physics2D.OverlapCircleAll(holdPoint.position, pickupRange);

        if (carriedItem == null)
        {
            foreach (var hit in itemHits)
            {
                EquationSlot slot = hit.GetComponent<EquationSlot>();
                if (slot != null && slot.isOccupied)
                {
                    WeightObject taken = slot.RemoveItem();
                    if (taken != null) { PickUp(taken); return; }
                }

                WeightObject item = hit.GetComponent<WeightObject>();
                if (item != null) { PickUp(item); return; }
            }
        }
        else
        {
            foreach (var hit in itemHits)
            {
                EquationSlot slot = hit.GetComponent<EquationSlot>();
                if (slot != null && !slot.isOccupied) { PlaceIntoSlot(slot); return; }
            }
            Drop();
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
        Vector3 worldPos = carriedItem.transform.position;
        carriedItem.transform.SetParent(null);
        carriedItem.transform.position = worldPos;

        Rigidbody2D itemRb = carriedItem.GetComponent<Rigidbody2D>();
        if (itemRb != null)
        {
            itemRb.isKinematic = false;
            itemRb.gravityScale = 0;
            itemRb.velocity = Vector2.zero;
        }

        carriedItem.transform.position += (Vector3)lastMoveDirection * 0.2f;
        carriedItem.GetComponent<SpriteRenderer>().sortingOrder = normalSortingOrder;
        carriedItem = null;
        ResetJumpParameters();
    }

    // Вспомогательные методы (Jump, Fall и т.д.) - без изменений
    IEnumerator JumpRoutine()
    {
        isJumping = true;
        anim?.SetTrigger("Jump");
        float jumpTime = currentJumpDuration / gravityMultiplier;
        float jumpH = currentJumpHeight / gravityMultiplier;
        float half = jumpTime / 2f; float t = 0;
        while (t < half) { t += Time.deltaTime; visual.localPosition = visualStartPos + Vector3.up * Mathf.Lerp(0, jumpH, t / half); yield return null; }
        t = 0;
        while (t < half) { t += Time.deltaTime; visual.localPosition = visualStartPos + Vector3.up * Mathf.Lerp(jumpH, 0, t / half); yield return null; }
        visual.localPosition = visualStartPos;
        isJumping = false; if (!IsGroundUnder()) StartFall();
    }

    void FixedUpdate()
    {
        if (isDead || MineGridManager2D.IsUIOpen) return;
        Vector2 dir = isFalling ? (Vector2)fallVelocity : (isJumping ? jumpDirection : moveInput);
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

    IEnumerator FallRoutine()
    {
        isFalling = true; anim?.SetTrigger("Fall");
        float visualProgress = 0; float deathTimer = 0;
        while (deathTimer < maxFallTime)
        {
            deathTimer += Time.deltaTime;
            visualProgress += (Time.deltaTime / maxFallTime) * gravityMultiplier;
            visualProgress = Mathf.Clamp01(visualProgress);
            float y = Mathf.Lerp(0, -fallDepth, visualProgress);
            visual.localPosition = visualStartPos + Vector3.up * y;
            if (IsGroundUnder()) { anim?.SetTrigger("Land"); EndFall(); yield break; }
            yield return null;
        }
        yield return StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        isDead = true; anim?.SetTrigger("Death");
        yield return new WaitForSeconds(deathAnimTime);
        if (CheckpointData.HasCheckpoint) uiManager.ShowDeathScreen(); else uiManager.GameOver();
    }

    void EndFall() { visual.localPosition = visualStartPos; sprite.sortingOrder = normalSortingOrder; isFalling = false; fallVelocity = Vector2.zero; }

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
            CheckpointData.HasCheckpoint = true; lastSafePosition = collision.transform.position;
            if (checkpointSound != null && SoundManager.instance != null) SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>()?.SetTrigger("appear");
        }
    }

    public void SetJumpParameters(float height, float duration)
    {
        currentJumpHeight = height; currentJumpDuration = duration;
        if (carriedItem != null) currentJumpHeight -= (carriedItem.weight * 0.05f);
    }

    public void ResetJumpParameters()
    {
        currentJumpHeight = defaultJumpHeight; currentJumpDuration = defaultJumpDuration;
        if (carriedItem != null) currentJumpHeight -= (carriedItem.weight * 0.05f);
    }

    private void PlaceIntoSlot(EquationSlot slot)
    {
        WeightObject itemToPlace = carriedItem; carriedItem = null;
        ResetJumpParameters(); slot.InsertItem(itemToPlace);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        if (holdPoint != null) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(holdPoint.position, pickupRange); }
        if (minePointer != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(minePointer.position, 0.2f); }
    }
}