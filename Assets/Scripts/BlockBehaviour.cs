using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private Animator anim;

    public bool isBreakable;
    public GameObject element;
    public int quantity;

    private bool movingUp = false;
    private bool movingDown = false;
    public bool deadly = false;
    private float maxHeight = 0.5f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (movingUp && maxHeight>0)
        {
            transform.Translate(Vector3.up * 0.1f);
            maxHeight -= 0.1f;
            
        }
        else if (movingUp)
        {
            movingUp = false;
            movingDown = true;
        }
        if (movingDown && maxHeight<0.5f)
        {
            transform.Translate(Vector3.down * 0.1f);
            maxHeight += 0.1f;
        }
        else if (movingDown)
        {
            movingDown = false;
        }

        if(!movingUp && !movingDown) deadly = false;
    }

    void Break()
    {
        FindObjectOfType<AudioManager>().Play("BreakBlock");
        float y = 6;
        for (int i=0; i<4; i++)
        {
            float x = 1;
            if (i % 2 == 1) x = -1;
            if (i == 2) y = 4f;
            GameObject debris = Instantiate(element, transform.position, Quaternion.identity);
            debris.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y), ForceMode2D.Impulse);
            Destroy(debris, 6f);
        }
        Destroy(gameObject);
    }

    void Bounce()
    {
        if(quantity<=0)
            FindObjectOfType<AudioManager>().Play("Bump");
        movingUp = true;
        deadly = true;
    }


    void HitBlock(int powerup)
    {
        if(isBreakable && powerup > 0)
        {
            Bounce();
            Break();
        } 
        else if (isBreakable)
        {
            Bounce();
        } 
        else
        {
            anim.enabled = true;
            if(quantity > 0)
            {
                Bounce();
                GameObject item = Instantiate(element, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                if(item.GetComponent<Coin>() != null)
                {
                    item.GetComponent<Coin>().MoveCoin();
                }
                
                quantity--;
            }
            if(quantity == 0)
            {
                anim.SetBool("isEmpty", true);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = collision.transform.position - transform.position;
            if (Vector2.Dot(direction.normalized, Vector2.down) >= 0.8f)
            {
                HitBlock(collision.gameObject.GetComponent<PlayerBehaviour>().powerUp);
            }
        }
    }
}
