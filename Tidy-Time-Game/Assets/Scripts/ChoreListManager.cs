using UnityEngine;

public class ChoresListManager : MonoBehaviour
{
    // Reference to the chores list panel
    public GameObject choresListPanel;

    // Reference to the player's movement script
    public PlayerScript playerMovement;

    // Open the chores list panel and disable player movement
    public void OpenChoreList()
    {
        if (choresListPanel != null)
        {
            choresListPanel.SetActive(true);
            if (playerMovement != null)
            {
                playerMovement.enabled = false; // Disable player movement
                playerMovement.StopMovement(); // Call a method to stop movement
            }
        }
        else
        {
            Debug.LogError("Chores List Panel is not assigned!");
        }
    }

    // Close the chores list panel and enable player movement
    public void CloseChoreList()
    {
        if (choresListPanel != null)
        {
            choresListPanel.SetActive(false);
            if (playerMovement != null)
            {
                playerMovement.enabled = true; // Enable player movement
            }
        }
        else
        {
            Debug.LogError("Chores List Panel is not assigned!");
        }
    }
}