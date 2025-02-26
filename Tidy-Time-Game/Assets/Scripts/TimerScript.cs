using UnityEngine;
using TMPro;
using System.Collections;

public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timeText; // Reference to the TextMeshProUGUI component
    private int currentHour = 6; // Start at 6 PM
    private int currentMinute = 0; // Start at 0 minutes
    private bool isRunning = true; // Flag to control the timer
    private bool isPaused = false; // Flag to pause the timer

    void Start()
    {
        if (timeText == null)
        {
            Debug.LogError("Time Text (TextMeshPro) is not assigned!");
            return;
        }

        // Initialize the text to "6:00 PM"
        UpdateTimeText();

        // Start the timer coroutine
        StartCoroutine(UpdateTimer());
    }

    // Coroutine to update the timer
    IEnumerator UpdateTimer()
    {
        while (isRunning)
        {
            if (!isPaused) // Only update the timer if it's not paused
            {
                yield return new WaitForSeconds(1); // Wait for 6 seconds

                // Increment the time by 1 minute
                currentMinute++;
                if (currentMinute >= 60)
                {
                    currentMinute = 0;
                    currentHour++;
                }

                // Stop the timer if it reaches 8:00 PM
                if (currentHour >= 8 && currentMinute >= 0)
                {
                    isRunning = false;
                    Debug.Log("Timer stopped at 8:00 PM.");
                }

                // Update the text
                UpdateTimeText();
            }
            else
            {
                yield return null; // Wait for the next frame if paused
            }
        }
    }

    // Update the text to display the current time
    private void UpdateTimeText()
    {
        string period = "PM"; // Since we're starting at 6 PM
        string minuteString = currentMinute.ToString("00"); // Ensure 2-digit format
        timeText.text = $"{currentHour}:{minuteString} {period}";
    }

    // Method to pause the timer
    public void PauseTimer()
    {
        isPaused = true;
    }

    // Method to resume the timer
    public void ResumeTimer()
    {
        isPaused = false;
    }
}