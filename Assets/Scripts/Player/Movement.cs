using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;

    Touch touch;
    Rigidbody rb;
    NavMeshAgent navMeshAgent;

    float width;
    float xPos;
    float yPos = 0f;
    float zPos = 1f;

    void Awake()
    {
        xPos = 0f;
        width = (float)Screen.width / 2f;

        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        TouchInput();
    }

    void FixedUpdate() 
    {
        PlayerMove();
    }

    void TouchInput()
    {
        if (Input.touchCount > 0)   //touch the screen with one finger at least
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pos = touch.position;
                xPos = (pos.x - width) / width;
            }

            else
            {
                xPos = 0f;   //otherwise it keeps moving even if you don't touch it
            }
        }
    }

    void PlayerMove()
    {
        Vector3 playerMove = new Vector3(xPos, yPos, zPos);
        rb.MovePosition(transform.position + (playerMove * moveSpeed * Time.fixedDeltaTime));
    }
}