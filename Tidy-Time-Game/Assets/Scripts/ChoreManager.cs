using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoreManager : MonoBehaviour
{
    // Boolean flags to track completion of each chore
    public bool isAlphabetSoupCompleted = false;
    public bool isSwapPlushiesCompleted = false;
    public bool isMathHomeworkCompleted = false;
    public bool isGarbageCompleted = false;
    public bool isOrganizeClosetCompleted = false;

    // Reference to the GameOver UI or logic
    public GameObject gameOverScreen; // Assign a UI panel or object in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the game over screen as inactive
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if all chores are completed
        if (AreAllChoresCompleted())
        {
            // END GAME
        }
    }

    // Method to check if all chores are completed
    private bool AreAllChoresCompleted()
    {
        return isAlphabetSoupCompleted &&
               isSwapPlushiesCompleted &&
               isMathHomeworkCompleted &&
               isGarbageCompleted &&
               isOrganizeClosetCompleted;
    }

    // Method to end the game
    private void EndGame()
    {
        // END GAME
    }

    // Public method to mark a chore as completed
    public void CompleteChore(string choreName)
    {
        switch (choreName)
        {
            case "AlphabetSoup":
                isAlphabetSoupCompleted = true;
                break;
            case "SwapPlushies":
                isSwapPlushiesCompleted = true;
                break;
            case "MathHomework":
                isMathHomeworkCompleted = true;
                break;
            case "Garbage":
                isGarbageCompleted = true;
                break;
            case "OrganizeCloset":
                isOrganizeClosetCompleted = true;
                break;
            default:
                Debug.LogWarning("Unknown chore: " + choreName);
                break;
        }

        Debug.Log(choreName + " completed!");
    }
}