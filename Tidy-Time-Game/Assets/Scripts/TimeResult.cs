using UnityEngine;
using TMPro;

public class TimeResult : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI timeResultText; // Assign in inspector

    void Start()
    {
        // Verify text component exists
        if (timeResultText == null)
        {
            Debug.LogError("TimeResult: Missing TextMeshProUGUI reference!");
            timeResultText = GetComponent<TextMeshProUGUI>();
            if (timeResultText == null) return;
        }

        DisplayCompletionTime();
    }

    void DisplayCompletionTime()
    {
        if (DataManager.Instance == null)
        {
            timeResultText.text = "Time data not available";
            Debug.LogWarning("DataManager instance missing!");
            return;
        }

        // Get the saved completion time
        var (hour, minute, second) = DataManager.Instance.GetTime();
        
        // Format the time string (e.g., "6:30:15 PM")
        string formattedTime = $"{hour}:{minute:D2}:{second:D2} PM";
        
        // Display the result
        timeResultText.text = $"{formattedTime}";
    }
}