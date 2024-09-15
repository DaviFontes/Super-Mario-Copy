using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;
    private SpriteRenderer sprite;
    private LayerMask layerMask;
    private Vector2 direction = Vector2.left;
    public GameObject shell;

    public float speed = 0.04f;
    private bool deathByShell = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        layerMask = LayerMask.GetMask("Player");
        enabled = false;
    }

    void FixedUpdate()
    {
        Move();
        if(Physics2D.Raycast(new Vector2(transform.position.x + (col.bounds.extents.x + 0.05f) * direction.x, transform.position.y), direction, 0.02f, layerMask))
        {
            
        }
        else if(Physics2D.Raycast(new Vector2(transform.position.x + (col.bounds.extents.x + 0.05f)*direction.x, transform.position.y), direction, 0.02f))
        {
            sprite.flipX = !sprite.flipX;
            direction = -direction;
        }
    }

    void Move()
    {
        rb.transform.Translate(direction * speed);
    }

    public void Die()
    {
        sprite.sortingLayerID = SortingLayer.NameToID("Particle");
        if (deathByShell)
        {
            FindObjectOfType<AudioManager>().Play("Kick");
            col.enabled = false;
            anim.enabled = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 2f;
            rb.AddForce(new Vector2(0f, 10f), ForceMode2D.Impulse);
            sprite.flipY = true;
            Destroy(gameObject, 6f);
        }
        else if (shell == null)
        {
            direction *= 0;
            anim.SetTrigger("isDead");
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            col.enabled = false;
            Destroy(gameObject, 0.75f);
        }
        else
        {
            Instantiate(shell, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }            
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerBehaviour>().starMan)
            {
                deathByShell = true;
                FindObjectOfType<GameManager>().AddScore(100, collision.transform.position);
                Die();
            }
        }
        if (collision.gameObject.CompareTag("Shell"))
        {
            if (collision.gameObject.GetComponent<ShellBehaviour>().isMoving){
                deathByShell = true;
                Die();
            }                
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if(collision.gameObject.GetComponent<BlockBehaviour>() != null)
            {
                if (collision.gameObject.GetComponent<BlockBehaviour>().deadly)
                {
                    deathByShell = true;
                    Die();
                }
            }
        }
    }

    private void OnBecameVisible()
    {
        gameObject.tag = "Enemy";
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        gameObject.tag = "Untagged";
        enabled = false;
    }

    private void OnEnable()
    {
        rb.WakeUp();
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
        rb.Sleep();
    }
}
