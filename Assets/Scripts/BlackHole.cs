using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Physics2D.IgnoreCollision(collision.collider, gameObject.GetComponent<BoxCollider2D>(), true);
            Destroy(collision.gameObject, 2f);
        }
    }
}
