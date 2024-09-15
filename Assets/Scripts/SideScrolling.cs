using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrolling : MonoBehaviour
{
    private Transform player;
    private float height;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        height = transform.position.y;
    }


    private void LateUpdate()
    {
        if (player)
        {
            Vector3 cameraPos = transform.position;
            cameraPos.x = Mathf.Max(cameraPos.x, player.position.x);
            cameraPos.y = Mathf.Max(height, player.position.y - 1.5f);
            transform.position = cameraPos;
        }
    }
}
