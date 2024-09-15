using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private LayerMask groundMask;
    private Vector2 direction = Vector2.right;

    public float speed = 4f;
    private bool isMovingUp;
    private bool isMoving;
    private bool isGrounded = false;
    private float maxHeight = 1f;
    private float jumpForce = 6f;
    private float rayOffsetX;
    private float rayOffsetY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        groundMask = LayerMask.GetMask("Ground");

        rayOffsetX = (col.size.x - 0.08f) / 2f;
        rayOffsetY = (col.size.y / 2f) + 0.01f;
    }

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("PowerUpSpawn");
        isMovingUp = true;
        isMoving = false;
    }

    void FixedUpdate()
    {
        if (isMovingUp && maxHeight > 0)
        {
            transform.Translate(Vector3.up * 0.08f);
            maxHeight -= 0.08f;
        }
        else
        {
            isMovingUp = false;
            col.enabled = true;
            rb.gravityScale = 1f;
            isMoving = true;
        }

        if (Physics2D.Raycast(new Vector2(transform.position.x + (col.bounds.extents.x + 0.02f) * direction.x, transform.position.y), direction, 0.05f, groundMask))
        {
            direction = -direction;
        }

        CheckCollision();
        if (isMoving)
        {
            if(isGrounded)
            {
                jumpForce = 6f;
            } else
            {
                jumpForce = 0f;
            }
            Move();
        }
            
    }

    void Move()
    {
        rb.velocity = new(0f, rb.velocity.y);
        rb.AddForce(new Vector2(direction.x * speed, jumpForce), ForceMode2D.Impulse); ;
        isGrounded = false;
    }

    void CheckCollision()
    {
        RaycastHit2D check1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayOffsetY), -Vector2.up, 0.02f, groundMask);
        RaycastHit2D check2 = Physics2D.Raycast(new Vector2(transform.position.x + (rayOffsetX), transform.position.y - rayOffsetY), -Vector2.up, 0.02f, groundMask);
        RaycastHit2D check3 = Physics2D.Raycast(new Vector2(transform.position.x - (rayOffsetX), transform.position.y - rayOffsetY), -Vector2.up, 0.02f, groundMask);

        if (check1 || check2 || check3)
        {
            isGrounded = true;            
        }

    }
}
