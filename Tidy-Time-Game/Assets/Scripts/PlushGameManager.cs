using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushGameManager : MonoBehaviour
{
    public static PlushGameManager Instance;
    public PlushHeadSwap[] heads; 
    public GameObject completionPanel; // Reference to your completion panel UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Check if the chore is already completed on launch
        CheckChoreCompletionOnStart();
    }

    private void CheckChoreCompletionOnStart()
    {
        ChoreManager choreManager = FindObjectOfType<ChoreManager>();
        if (choreManager != null && choreManager.isSwapPlushiesCompleted)
        {
            // If the chore is already completed, show the completion panel
            if (completionPanel != null)
            {
                completionPanel.SetActive(true);
            }
        }
        else
        {
            // Ensure the panel is hidden if the chore isn't completed
            if (completionPanel != null)
            {
                completionPanel.SetActive(false);
            }
        }
    }

    public void CheckTaskCompletion()
    {
        bool allCorrect = true;

        foreach (PlushHeadSwap head in heads)
        {
            if (!head.IsCorrectlyPlaced())
            {
                Debug.Log($"{head.name} is not in the correct position.");
                allCorrect = false;
            }
            else
            {
                Debug.Log($"{head.name} is correctly placed.");
            }
        }

        if (allCorrect)
        {
            Debug.Log("All plushies matched correctly");

            // Show the completion panel if it exists
            if (completionPanel != null)
            {
                completionPanel.SetActive(true);
            }

            // Mark the plushie swap as completed
            ChoreManager choreManager = FindObjectOfType<ChoreManager>();
            if (choreManager != null)
            {
                choreManager.CompleteChore("SwapPlushies");
            }
        }
        else
        {
            // Optional: Hide the panel if not all are correct
            if (completionPanel != null)
            {
                completionPanel.SetActive(false);
            }
        }
    }
}