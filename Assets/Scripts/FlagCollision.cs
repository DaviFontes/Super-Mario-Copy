using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCollision : MonoBehaviour
{
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreLayerCollision(7, 11, true);
            StartCoroutine(collision.gameObject.GetComponent<PlayerMovement>().FinishAnimation());
            anim.SetTrigger("Down");
            StartCoroutine(FindObjectOfType<GameManager>().StageClear());
        }
    }
}
