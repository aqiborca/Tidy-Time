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
    public TimerScript timerScript; // Reference to the TimerScript
    private Vector2 moveDirection;
    private Vector2 currentVelocity = Vector2.zero;
    private bool canMove = true; // Flag to control movement
    Animator animator;
    private Vector3 originalScale; // Store the original scale of the sprite

    void Start()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale; // Store the initial scale
    }

    // Get player position and time when loading scene
    private void OnEnable()
    {
        GetPlayerPosition();
        GetSavedTime();
    }

    // Set player position and time when changing scene
    private void OnDisable()
    {
        SetPlayerPosition();
        SaveCurrentTime();
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
        if (enabled && canMove && Time.timeScale > 0) // Only move if the script is enabled, movement is allowed, and game isn't paused
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

        // Set isWalking parameter based on player movement
        bool isWalking = moveDirection.magnitude > 0;
        animator.SetBool("isWalking", isWalking); // Set the isWalking parameter in the animator

        // Flip sprite based on horizontal movement direction
        if (moveX > 0) // Moving right
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }
        else if (moveX < 0) // Moving left
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        if (isWalking)
        {
            //Type here
        }
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
            // Get current position but force Z=70
            Vector3 positionToSave = new Vector3(transform.position.x, transform.position.y, 70);
            DataManager.Instance.SetPlayerPosition(positionToSave);
        }
    }
    // Player Position Getter
    private void GetPlayerPosition()
    {
        // Default position with Z=70
        Vector3 defaultPosition = new Vector3(-5, 0, 70);

        // Load the player's position from the DataManager
        if (DataManager.Instance != null)
        {
            Vector3 savedPosition = DataManager.Instance.GetPlayerPosition();

            // Only use saved X and Y coordinates, force Z=70
            transform.position = new Vector3(savedPosition.x, savedPosition.y, 70);
        }
        else
        {
            // If no DataManager, use default position with Z=70
            transform.position = defaultPosition;
        }
    }
    // Save the current time before changing scenes
    private void SaveCurrentTime()
    {
        if (DataManager.Instance != null && timerScript != null)
        {
            DataManager.Instance.SetTime(timerScript.GetCurrentHour(), timerScript.GetCurrentMinute(), timerScript.GetCurrentSecond());
        }
    }

    // Load the saved time when entering a new scene
    private void GetSavedTime()
    {
        if (DataManager.Instance != null && timerScript != null)
        {
            var savedTime = DataManager.Instance.GetTime();
            timerScript.SetTime(savedTime.Item1, savedTime.Item2, savedTime.Item3);
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

            // Pause or unpause the game
            Time.timeScale = isPanelActive ? 0f : 1f;

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
    private void OnDestroy()
    {
        // When the player object is destroyed (like when changing scenes),
        // make sure we're not leaving the game in a paused state
        if (escPanel != null && escPanel.activeSelf)
        {
            escPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}