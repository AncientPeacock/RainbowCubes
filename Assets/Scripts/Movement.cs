using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    //Vector3 horizontalMove;
    Vector3 forwardMove;

    Touch touch;
    Vector3 position;
    float width;
    float height;

    //Vector2 forwardMove;


    void Awake()
    {
        width = (float)Screen.width / 2f;
        height = (float)Screen.height / 2f;

        position = new Vector3(0f, 0f, 0f);
    }

    void Update()
    {
        //float horizontalInput = Input.GetAxis("Horizontal");
        //horizontalMove = Vector3.right * horizontalInput;
        //forwardMove = Vector3.forward * moveSpeed;
        //Vector3 playerMove = forwardMove + horizontalMove;

        //transform.Translate(playerMove * Time.deltaTime);

        //forwardMove = Vector2.up * moveSpeed * Time.deltaTime;
        //forwardMove = Vector3.forward * moveSpeed * Time.deltaTime;
        //forwardMove = Vector3.forward * moveSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        if (Input.touchCount > 0)   //touch the screen with one finger at least
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pos = touch.position;
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;
                position = new Vector3(pos.x, 0f, 0f);

                // Position the cube.
                transform.position = position;

                

                //transform.position = new Vector3(transform.position.x + touch.deltaPosition.x * moveSpeed, transform.position.y, transform.position.z + touch.deltaPosition.y * moveSpeed);

                
                //transform.position = new Vector3(touch.position.x, transform.position.y, forwardMove.z);
            }

        }
    }
}
