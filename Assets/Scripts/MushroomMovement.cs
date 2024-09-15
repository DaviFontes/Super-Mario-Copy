using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private LayerMask groundMask;
    private Vector2 direction = Vector2.right;

    public float speed = 0.04f;
    private bool isMovingUp;
    private bool isMoving;
    private float maxHeight = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;
        groundMask = LayerMask.GetMask("Ground");
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
        } else
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

        if(isMoving)
            Move();
    }

    void Move()
    {
        rb.transform.Translate(direction * speed);
    }
}
