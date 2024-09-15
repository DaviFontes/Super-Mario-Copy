using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ShellBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sprite;
    private LayerMask groundMask;
    private Vector2 direction = Vector2.left;


    public bool isMoving;
    public float speed = 0.06f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        groundMask = LayerMask.GetMask("Ground");
        sprite = GetComponent<SpriteRenderer>();

        isMoving = false;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            if (Physics2D.Raycast(new Vector2(transform.position.x + (col.bounds.extents.x + 0.02f) * direction.x, transform.position.y), direction, 0.05f, groundMask))
            {
                direction = -direction;
            }
            MoveShell();
        }
    }

    public void MoveShell()
    {
        rb.transform.Translate(direction * speed);
    }

    public void KillShell()
    {
        sprite.sortingLayerID = SortingLayer.NameToID("Particle");
        sprite.flipY = true;
        col.enabled = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        rb.AddForce(new Vector2(0f, 10f), ForceMode2D.Impulse);
        Destroy(gameObject, 6f);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerBehaviour>().starMan)
            {
                KillShell();
                return;
            }
            if (collision.GetContact(0).point.x > col.bounds.center.x)
            {
                direction = Vector2.left;
            }
            else
            {
                direction = Vector2.right;
            }
        }
        if (collision.gameObject.CompareTag("Shell"))
        {
            KillShell();
            collision.gameObject.GetComponent<ShellBehaviour>().KillShell();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.gameObject.GetComponent<BlockBehaviour>() != null)
            {
                if (collision.gameObject.GetComponent<BlockBehaviour>().deadly)
                    KillShell();
            }
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
