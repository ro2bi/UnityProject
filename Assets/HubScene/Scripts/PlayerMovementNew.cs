using UnityEngine;
using System.Collections.Generic;

public class PlayerMovementNew : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim; // Должен быть получен в Start()

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // !!! ВАЖНО: Инициализация KeybindManager перед его использованием !!!
        // Это гарантирует, что словарь keybinds заполнен (либо по умолчанию, либо загруженными данными).
        KeybindManager.InitializeKeys();
    }

    void Update()
    {
        // === 1. Чтение Пользовательского Ввода через KeybindManager ===

        float horizontal = 0f;
        float vertical = 0f;

        // Горизонтальное движение
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_RIGHT)))
        {
            horizontal = 1f;
        }
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_LEFT)))
        {
            horizontal = -1f;
        }

        // Вертикальное движение (если нужно, используя стандартные W/S или другие клавиши, если они есть в KeybindManager)
        // В вашем KeybindManager нет отдельных констант для Up/Down, поэтому используем временные проверки.
        // Если вы планируете добавить константы MOVE_UP и MOVE_DOWN, используйте их здесь.
        // Пока что используем прямое считывание, если вы не переназначили вертикальные клавиши:
        if (Input.GetKey(KeyCode.W)) // Если W - Вперед
        {
            vertical = 1f;
        }
        else if (Input.GetKey(KeyCode.S)) // Если S - Назад
        {
            vertical = -1f;
        }

        // --- Если вы добавили константы MOVE_UP/MOVE_DOWN в KeybindManager ---
        /*
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_FORWARD))) 
        {
            vertical = 1f;
        }
        else if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_BACKWARD)))
        {
            vertical = -1f;
        }
        */

        // === 2. Обновление moveInput и Аниматора ===

        moveInput.x = horizontal;
        moveInput.y = vertical;
        moveInput.Normalize();

        // === Логика анимации (с использованием 4-х булевых параметров) ===

        // 1. Движение вперед/вверх (W или клавиша Вперед)
        anim.SetBool("IsMovingForward", vertical > 0);

        // 2. Движение назад/вниз (S или клавиша Назад)
        anim.SetBool("IsMovingBackward", vertical < 0);

        // 3. Движение вправо (D или клавиша Вправо)
        anim.SetBool("IsMovingRight", horizontal > 0);

        // 4. Движение влево (A или клавиша Влево)
        anim.SetBool("IsMovingLeft", horizontal < 0);


        // === Проверка для INTERACT (пример использования) ===
        if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.INTERACT)))
        {
            Debug.Log("Interact action triggered!");
            // Вызов метода взаимодействия
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}