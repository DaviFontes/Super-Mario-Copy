using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private GameManager gameManager;
    private bool isMoving = false;
    private float maxHeight = 1.5f; 

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (isMoving && maxHeight > 0)
        {
            transform.Translate(Vector3.up * 0.08f);
            maxHeight -= 0.08f;
        } else if (maxHeight <= 0)
        {
            CollectCoin();
        }
    }

    public void CollectCoin()
    {
        gameManager.AddCoin();
        FindObjectOfType<AudioManager>().Play("Coin");
        Destroy(gameObject);
    }

    public void MoveCoin()
    {
        isMoving = true;
    }
}
