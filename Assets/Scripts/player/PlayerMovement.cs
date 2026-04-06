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

    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;

    private Animator anim;


    private BoxCollider2D boxCollider;

    private float wallJumpCooldown;

    private float horizontalInput;

    [Header ("SFX")]
[SerializeField] private AudioClip JumpSound;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        KeybindManager.InitializeKeys();
    }

    private void Update()
    {
        
        horizontalInput = 0;
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_RIGHT)))
        {
            horizontalInput += 1;
        }
        if (Input.GetKey(KeybindManager.GetKey(KeybindManager.MOVE_LEFT)))
        {
            horizontalInput -= 1;
        }


        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", IsGrounded());

        if (wallJumpCooldown > 0.2f)
        {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (OnWall() && !IsGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }

            else
            {
                body.gravityScale = 3;
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
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }

        else if (OnWall() && !IsGrounded())
        {
           if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
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
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit2D.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && IsGrounded() && !OnWall();
    }
}
