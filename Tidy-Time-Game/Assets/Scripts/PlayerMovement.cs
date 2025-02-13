using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    public float moveSpeed;
    public Rigidbody2D playerRigidBody;
    private Vector2 moveDirection;
    // Inputs
    void Update()
    {
        ProcessInputs();
    }
    //Physics Calculations
    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs(){
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY); //
    }

    void Move(){
        playerRigidBody.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }
}
