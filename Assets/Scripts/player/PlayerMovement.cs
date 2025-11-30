using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [SerializeField] private LayerMask groundLayer;
    // groundLayer - это слой, на котором находится земля, по которой может ходить игрок
    // [SerializeField] - позволяет редактировать переменную в инспекторе Unity, но она остается приватной

    [SerializeField] private LayerMask wallLayer;
    // wallLayer - это слой, на котором находятся стены, о которые может упираться игрок

    private Rigidbody2D body;
    // доступ к Rigidbody2D - компонент, который отвечает за физику объекта в Unity

    private Animator anim;
    // доступ к Animator - компонент, который отвечает за анимацию объекта в Unity

    //private bool grounded;
    // grounded - переменная, которая будет использоваться для проверки, находится ли игрок на земле

    private BoxCollider2D boxCollider;
    // доступ к BoxCollider2D - компонент, который отвечает за столкновения объекта в Unity

    private float wallJumpCooldown;
    // wallJumpCooldown - переменная, которая будет использоваться для проверки, можно ли прыгнуть от стены

    private float horizontalInput;
    // horizontalInput - переменная, которая будет использоваться для проверки, нажаты ли клавиши влево или вправо

    [Header ("SFX")]
[SerializeField] private AudioClip JumpSound;

    //Awake - значит работает сразу при запуске игры
    //в этом методе мы получаем компонент Rigidbody2D, который отвечает за физику объекта
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // получаем body - Rigidbody2D компонент, который отвечает за физику объекта, берет инфу из Rigidbody2D
        anim = GetComponent<Animator>();
        // Получаем компонент anim, который отвечает за анимацию объекта, берет инфу из Animator
        boxCollider = GetComponent<BoxCollider2D>();
        // Получаем компонент boxCollider, который отвечает за столкновения объекта, берет инфу из BoxCollider2D

        KeybindManager.InitializeKeys();
    }

    //Update - работает так же как и Awake, но работает после каждого кадра (типо после нажатия чего то и тд.)
    private void Update()
    {
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        ////Input.GetAxis("Horizontal") - получает значение от -1 до 1 в зависимости от нажатых клавиш (A, D, стрелки влево и вправо)

        //// Поворачиваем игрока вправо/влево в зависимости от направления движения
        //if (horizontalInput > 0.01f)
        //{
        //    transform.localScale = Vector3.one;
        //}
        //else if (horizontalInput < -0.01f)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1);
        //}
        horizontalInput = 0;
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_RIGHT)))
        {
            horizontalInput += 1;
        }
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_LEFT)))
        {
            horizontalInput -= 1;
        }


        // назначаем параметры к анимации
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", IsGrounded());
        // anim.SetBool("Run", horizontalInput != 0) - если игрок движется, то анимация бега будет активна

        //тут реализована логика прыжка от стены:
        if (wallJumpCooldown > 0.2f)
        {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
                // если игрок на стене и не на земле, то он останавливается
            }

            else
            {
                body.gravityScale = 3;
                // если игрок не на стене, то он падает
            }

            if (Input.GetKey(KeybindManager.GetKey(KeybindManager.JUMP))) 
            {
                Jump();

                if (Input.GetKeyDown(KeybindManager.GetKey(KeybindManager.JUMP)) && IsGrounded()) 
                {
                    SoundManager.instance.PlaySound(JumpSound);
                }
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
            // Time.deltaTime - это время, прошедшее с последнего кадра, используется для плавности движения
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            // если игрок на земле, то он может прыгнуть
            anim.SetTrigger("jump");
            // anim.SetTrigger("Jump") - запускает анимацию прыжка
        }

        else if (OnWall() && !IsGrounded())
        {
           if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                // если игрок на стене и не на земле, то он может прыгнуть от стены, но без вертикальной скорости
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                // поворачиваем игрока в сторону стены, от которой он прыгает
            }
            // если игрок на стене и не на земле, то он может прыгнуть от стены
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }
            wallJumpCooldown = 0;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit2D.collider != null;
        // Physics2D.BoxCast - создает луч, который проверяет, находится ли игрок на земле
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit2D.collider != null;
        // Physics2D.BoxCast - создает луч, который проверяет, находится ли игрок на земле
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && IsGrounded() && !OnWall();
        // проверяем, может ли игрок атаковать: если он стоит на месте, на земле и не на стене
    }
}
