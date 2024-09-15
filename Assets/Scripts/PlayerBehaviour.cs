using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;
    private SpriteRenderer sprite;
    private GameManager gameManager;

    public int powerUp = 0;
    public bool starMan = false;
    private float starTime = 10f;
    private bool invulnerable = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (powerUp > 0) anim.SetBool("isBig", true);
        else anim.SetBool("isBig", false);

        if (starMan)
        {
            starTime -= Time.deltaTime;
            if (starTime <= 0)
            {
                starMan = false;
                FindObjectOfType<AudioManager>().PlayMusic("Theme");
                starTime = 10f;
            }
        }
    }

    public void TakeDamage()
    {
        if (!starMan && !invulnerable)
        {
            powerUp -= 1;
            
            if (powerUp < 0)
            {
                Die();
            } else
            {
                invulnerable = true;
                StartCoroutine(Flash(15));
            }
        }
    }

    IEnumerator Flash(int times)
    {
        for (int i = 0; i < times; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.05f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        invulnerable = false;

    }

    public void Die()
    {
        StartCoroutine(gameManager.PlayerDeath());        
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 2f;
        rb.AddForce(new Vector2(0f, 10f), ForceMode2D.Impulse);
        anim.SetTrigger("isDead");
        Destroy(gameObject, 6f);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!starMan)
            {
                Vector2 direction = collision.transform.position - transform.position;
                if (Vector2.Dot(direction.normalized, Vector2.down) >= 0.75f)
                {
                    collision.gameObject.GetComponent<EnemyMovement>().Die();
                    gameManager.AddScore(100, collision.transform.position);
                    GetComponent<PlayerMovement>().SetIsBouncing(true);
                }
                else
                {
                    DisableCollision(collision.collider, col);
                    TakeDamage();
                }
            }
        }
        if (collision.gameObject.CompareTag("Shell"))
        {
            if (collision.gameObject.GetComponent<ShellBehaviour>().isMoving)
            {
                Vector2 direction = collision.transform.position - transform.position;
                if (Vector2.Dot(direction.normalized, Vector2.down) >= 0.75f)
                {
                    collision.gameObject.GetComponent<ShellBehaviour>().isMoving = false;
                    GetComponent<PlayerMovement>().SetIsBouncing(true);                
                }
                else
                {
                    DisableCollision(collision.collider, col);
                    TakeDamage();
                }
            }
            else
            {
                StartCoroutine(DisableCollision());
                FindObjectOfType<AudioManager>().Play("Kick");
                collision.gameObject.GetComponent<ShellBehaviour>().isMoving = true;
            }
            
        }
        if (collision.gameObject.CompareTag("Coin"))
        {
            collision.gameObject.GetComponent<Coin>().CollectCoin();
        }
        if (collision.gameObject.CompareTag("Mushroom"))
        {
            if (powerUp == 0)
            {
                FindObjectOfType<AudioManager>().Play("PowerUp");
                powerUp++;
            }
            gameManager.AddScore(1000, collision.transform.position);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("FireFlower"))
        {
            if (powerUp == 1)
            {
                FindObjectOfType<AudioManager>().Play("PowerUp");
                powerUp++;
            }
            gameManager.AddScore(1000, collision.transform.position);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Star"))
        {
            starMan = true;
            StartCoroutine(Flash(75));
            FindObjectOfType<AudioManager>().PlayMusic("StarTheme");
            gameManager.AddScore(1000, collision.transform.position);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("1Up"))
        {
            gameManager.AddLife(collision.transform.position);
            Destroy(collision.gameObject);
        }
            if (collision.gameObject.CompareTag("Hole"))
        {
            Die();
        }
    }

    IEnumerator DisableCollision()
    {
        Physics2D.IgnoreLayerCollision(7, 9, true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(7, 9, false);
    }

    IEnumerator DisableCollision(Collider2D c1, Collider2D c2)
    {
        Physics2D.IgnoreCollision(c1, c2, true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(c1, c2, false);
    }

}
