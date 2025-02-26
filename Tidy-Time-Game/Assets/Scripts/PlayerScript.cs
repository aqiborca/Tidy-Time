/*
This class detects if a player is inside of a prop collider and allows them to press the respective button
Attached to: Player Object
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Variables
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public Rigidbody2D playerRigidBody;
    public GameObject escPanel; // Reference to the ESC Panel
    private Vector2 moveDirection;
    private Vector2 currentVelocity = Vector2.zero;
    private bool canMove = true; // Flag to control movement

    // Get player position when loading scene
    private void OnEnable()
    {
        GetPlayerPosition();
    }

    // Set player position when changing scene
    private void OnDisable()
    {
        SetPlayerPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled && canMove) // Only process inputs if the script is enabled and movement is allowed
        {
            ProcessInputs();
        }

        // Check if ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscPanel();
        }
    }

    void FixedUpdate()
    {
        if (enabled && canMove) // Only move if the script is enabled and movement is allowed
        {
            Move();
        }
    }

    // Inputs for player movement
    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    // Player movement
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

    // Player Position Setter
    private void SetPlayerPosition()
    {
        // Save the player's position to the DataManager
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SetPlayerPosition(transform.position);
        }
    }

    // Player Position Getter
    private void GetPlayerPosition()
    {
        // Load the player's position from the DataManager
        if (DataManager.Instance != null)
        {
            Vector3 savedPosition = DataManager.Instance.GetPlayerPosition();
            transform.position = savedPosition;
        }
    }

    // Method to stop movement
    public void StopMovement()
    {
        playerRigidBody.velocity = Vector2.zero; // Reset velocity
        moveDirection = Vector2.zero; // Clear movement input
        currentVelocity = Vector2.zero; // Reset current velocity
    }

    // Method to toggle the ESC Panel
    public void ToggleEscPanel()
    {
        if (escPanel != null)
        {
            bool isPanelActive = !escPanel.activeSelf;
            escPanel.SetActive(isPanelActive);

            // Stop or resume player movement
            if (isPanelActive)
            {
                canMove = false; // Disable movement
                StopMovement(); // Stop movement when panel is active
            }
            else
            {
                canMove = true; // Enable movement when panel is inactive
            }
        }
    }
}