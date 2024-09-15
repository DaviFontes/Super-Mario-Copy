using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerBehaviour behaviour;
    private GameManager gameManager;

    private Vector2 move;
    private bool facingRight = true;

    public float speed = 5f;
    public float jumpForce = 8f;
    public float gravityScale = 3f;
    private bool running = false;
    private float speedMult = 1f;

    private bool isGrounded = true;
    private bool isBouncing = false;
    private bool jumped = false;
    private bool stopJump = false;
    private bool startTimer = false;
    private float jumpTime = 0.5f;
    private bool finished = false;

    private LayerMask layerMask; 
    private float rayCastLengthCheck = 0.02f;
    private float rayOffsetX;
    private float rayOffsetY;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        behaviour = GetComponent<PlayerBehaviour>();
        gameManager = FindObjectOfType<GameManager>();
        layerMask = LayerMask.GetMask("Ground");

        rayOffsetX = (col.size.x - 0.08f) / 2f;
        rayOffsetY = (col.size.y / 2f) + 0.01f;
    }

    void Update()
    {
        if (finished) return;
        if (gameManager.isPaused) return;

        rayOffsetX = (col.size.x - 0.08f) / 2f;
        rayOffsetY = (col.size.y / 2f) + 0.01f;

        CheckCollision();
        rb.velocity = new Vector2(0, rb.velocity.y);

        move = new (Input.GetAxis("Horizontal") * speed * speedMult, 0);
        if(move.x == 0) anim.SetBool("isRunning", false);
        else anim.SetBool("isRunning", true);

        if (Input.GetButton("Fire1"))
        {
            running = true;
        } else
        {
            running = false;
        }

        if (running)
        {
            anim.speed = 3f;
            speedMult = 1.5f;
        }
        else
        {
            anim.speed = 1f;
            speedMult = 1;
        }

        if (move.x < 0 && facingRight)
        {
            sprite.flipX = true;
            facingRight = false;
        } else if (move.x > 0 && !facingRight)
        {
            sprite.flipX = false;
            facingRight = true;
        }

        if (Input.GetButton("Jump") && isGrounded)
        {
            jumped = true;
            anim.SetBool("isJumping", true);
            if(behaviour.powerUp > 0)
                FindObjectOfType<AudioManager>().Play("BigJump");
            else
                FindObjectOfType<AudioManager>().Play("SmallJump");
        } 
        if (Input.GetButton("Jump") && isBouncing)
        {
            isBouncing = false;
        }
        if(Input.GetButtonUp("Jump"))
        {
            stopJump = true;
        }
        if (startTimer)
        {
            jumpTime -= Time.deltaTime;
            if (jumpTime <= 0 || rb.velocity.y == 0) stopJump = true;
            if (jumpTime < 0.45f && isBouncing) stopJump = true;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(move * Time.fixedDeltaTime);

        if(jumped)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumped = false;
            startTimer = true;
        }
        if(stopJump)
        {
            rb.gravityScale = gravityScale;
            stopJump = false;
            isBouncing = false;
            startTimer = false;
            jumpTime = 0.5f;
        }
    }

    public void SetIsBouncing(bool value)
    {
        isBouncing = value;
        jumped = value;
        FindObjectOfType<AudioManager>().Play("Stomp");
    }

    void CheckCollision()
    {
        RaycastHit2D check1 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayOffsetY), -Vector2.up, rayCastLengthCheck, layerMask);
        RaycastHit2D check2 = Physics2D.Raycast(new Vector2(transform.position.x + (rayOffsetX), transform.position.y - rayOffsetY), -Vector2.up, rayCastLengthCheck, layerMask);
        RaycastHit2D check3 = Physics2D.Raycast(new Vector2(transform.position.x - (rayOffsetX), transform.position.y - rayOffsetY), -Vector2.up, rayCastLengthCheck, layerMask);
       
        if (check1 || check2 || check3)
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        } else
        {
            isGrounded = false;
        }

    }

    public IEnumerator FinishAnimation()
    {
        finished = true;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.8f;
        yield return new WaitForSeconds(1.7f);
        transform.Translate(new Vector3(1, 0, 0));
        yield return new WaitForSeconds(0.3f);
        rb.AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);

        if (behaviour.powerUp > 0)
            FindObjectOfType<AudioManager>().Play("BigJump");
        else
            FindObjectOfType<AudioManager>().Play("SmallJump");

        move = new(speed / 5, 0);
        yield return new WaitForSeconds(1.3f);
        anim.SetBool("isJumping", false);
        anim.SetBool("isRunning", true);
        anim.speed = 0.4f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            sprite.enabled = false;
        }
    }

}
