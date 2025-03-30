using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorPanelsManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The main UI panel that shows by default")]
    public GameObject defaultPanel;
    [Tooltip("The button that appears when all chores are complete")]
    public GameObject completionButton;

    private void Start()
    {
        // Initialize the UI state
        UpdateUIState();
        
        // Subscribe to chore completion events (optional optimization)
        ChoreManager.Instance.OnAllChoresCompleted += HandleAllChoresCompleted;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (ChoreManager.Instance != null)
        {
            ChoreManager.Instance.OnAllChoresCompleted -= HandleAllChoresCompleted;
        }
    }

    private void HandleAllChoresCompleted()
    {
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        bool allChoresComplete = ChoreManager.Instance.AreAllChoresCompleted();
        
        defaultPanel.SetActive(!allChoresComplete);
        completionButton.SetActive(allChoresComplete);
    }
}