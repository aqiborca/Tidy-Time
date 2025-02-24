using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public Rigidbody2D playerRigidBody;
    private Vector2 moveDirection;
    private Vector2 currentVelocity = Vector2.zero;

    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            currentVelocity = moveDirection * maxSpeed;
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        playerRigidBody.velocity = currentVelocity;
    }
}
