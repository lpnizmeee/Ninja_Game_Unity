using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;
    private bool isDeath = false;

    private float horizontal;
    private int coin = 0;

    private Vector3 savePoint;

    void Start()
    {
        SavePoint();
    }

    void FixedUpdate()
    {
        if (isDeath)
        {
            return;
        }

        isGrounded = CheckGrounded();

        // -1 -> 0 -> 1 
        horizontal = Input.GetAxisRaw("Horizontal");
        // vertical = Input.GetAxisRaw("Vetical");

        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            //jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }


            //change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded) Attack();
            //throw
            if (Input.GetKeyDown(KeyCode.V) && isGrounded) Throw();
        }
        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }
        
        //Moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * Time.fixedDeltaTime * speed, rb.velocity.y);

            transform.rotation = quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : math.PI, 0));
            //transform.localScale = new Vector3(horizontal, 1, 1);
        }
        //Idle
        else if (isGrounded)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }

    }

    public override void OnInit()
    {
        base.OnInit();
        isDeath = false;
        isAttack = false;

        transform.position = savePoint;
        ChangeAnim("idle");
    }


    public override void OnDespawn()
    {
        base.OnDespawn();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        return hit.collider != null;
    }
    private void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }
    private void Throw()
    {
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void ResetAttack()
    {
        ChangeAnim("idle");
        isAttack = false;
    }
    private void Jump()
    {
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }

    
    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coin++;
            Destroy(collision.gameObject);
        }

        if (collision.tag == "DeathZone")
        {
            isDeath = true;
            ChangeAnim("die");

            Invoke(nameof(OnInit), 1f);
        }
    }

}
