/*
This script is for counting and saving timer
Attached to: TimerManager Object
*/
using UnityEngine;
using TMPro;
using System.Collections;

public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timeText; // Reference to the TextMeshProUGUI component
    private int currentHour = 6;     // Start at 6 PM
    private int currentMinute = 0;   // Start at 0 minutes
    private int currentSecond = 0;   // Start at 0 seconds
    private bool isRunning = true;   // Flag to control the timer
    private bool isPaused = false;   // Flag to pause the timer

    private static TimerScript instance;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene changes
            ReassignTimeText(); // Assign timeText using the "Timer" tag
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Ensure the timeText is assigned
        if (timeText == null)
        {
            ReassignTimeText();
        }

        if (timeText == null)
        {
            return;
        }

        // Initialize the text to "6:00:00 PM"
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
                yield return new WaitForSeconds(0.5f); // Wait for 0.5 second

                // Increment the time by 30 seconds
                currentSecond += 30;
                if (currentSecond >= 60)
                {
                    currentSecond = 0;
                    currentMinute++;
                }
                if (currentMinute >= 60)
                {
                    currentMinute = 0;
                    currentHour++;
                }

                // Stop the timer if it reaches 8:00:00 PM
                if (currentHour >= 8 && currentMinute >= 0 && currentSecond >= 0)
                {
                    isRunning = false;
                    Debug.Log("Timer stopped at 8:00:00 PM.");
                }

                // Update the UI text
                UpdateTimeText();
            }
            else
            {
                yield return null; // Wait for the next frame if paused
            }
        }
    }

    // Returns the current hour (used for saving)
    public int GetCurrentHour()
    {
        return currentHour;
    }

    // Returns the current minute (used for saving)
    public int GetCurrentMinute()
    {
        return currentMinute;
    }

    // Returns the current second (used for saving)
    public int GetCurrentSecond()
    {
        return currentSecond;
    }

    // Sets the timer to a saved time (used when loading a scene)
    public void SetTime(int hour, int minute, int second, TextMeshProUGUI newTimeText = null)
    {

        // Stop running coroutines to prevent duplicates
        StopAllCoroutines();

        // Update time variables
        currentHour = hour;
        currentMinute = minute;
        currentSecond = second;
        isRunning = true;

        // If a new UI reference is given, update it
        if (newTimeText != null)
        {
            timeText = newTimeText;
        }
        else
        {
            ReassignTimeText(); // Reassign timeText if no new reference is provided
        }

        if (timeText == null)
        {
            return;
        }

        // Update the UI text
        UpdateTimeText();

        // Restart the timer coroutine
        StartCoroutine(UpdateTimer());
    }

    // Updates the UI text with the current time
    private void UpdateTimeText()
    {
        if (timeText == null)
        {
            ReassignTimeText(); // Attempt to reassign timeText if it's null
        }

        if (timeText != null)
        {
            timeText.text = $"{currentHour}:{currentMinute:D2}:{currentSecond:D2} PM";
        }
    }

    // Pauses the timer
    public void PauseTimer()
    {
        isPaused = true;
    }

    // Resumes the timer
    public void ResumeTimer()
    {
        isPaused = false;
    }

    // Reassigns the timeText reference by searching for the GameObject with the "Timer" tag
    private void ReassignTimeText()
    {
        GameObject textObject = GameObject.FindWithTag("Timer");
        if (textObject != null)
        {
            timeText = textObject.GetComponent<TextMeshProUGUI>();
            if (timeText == null)
            {
            }
        }
    }
}