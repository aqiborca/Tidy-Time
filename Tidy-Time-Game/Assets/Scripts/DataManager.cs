/*
This script allows for saving data between scenes
Attached to: Data Manager Object
*/
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Variables
    public static DataManager Instance { get; private set; }
    public Vector3 playerPosition;
    public int currentHour = 6;  // Default time at 6 PM
    public int currentMinute = 0;

    private void Awake()
    {
        // Ensure only one instance of DataManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Player Position Setter
    public void SetPlayerPosition(Vector3 position)
    {
        playerPosition = position;
    }

    // Player Position Getter
    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }

    // Time Setter
    public void SetTime(int hour, int minute)
    {
        currentHour = hour;
        currentMinute = minute;
    }

    // Time Getter
    public (int, int) GetTime()
    {
        return (currentHour, currentMinute);
    }
}