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
    public int currentHour = 4;
    public int currentMinute = 0;
    public int currentSecond = 0; // Added seconds

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
        // If we have the "no position" marker, return default position with Z=70
        if (playerPosition == new Vector3(-999, -999, -999))
        {
            return new Vector3(-5, 0, 70);
        }

        // Ensure saved position always has Z=70
        return new Vector3(playerPosition.x, playerPosition.y, 70);
    }

    // Time Setter
    public void SetTime(int hour, int minute, int second)
    {
        currentHour = hour;
        currentMinute = minute;
        currentSecond = second;
    }

    // Time Getter
    public (int, int, int) GetTime()
    {
        return (currentHour, currentMinute, currentSecond);
    }

    public void ClearPlayerPosition()
    {
        // Set to default position with Z=70 when clearing
        playerPosition = new Vector3(-5, 0, 70);
    }

    public void ClearAllData()
    {
        playerPosition = Vector3.zero;
        currentHour = 4;
        currentMinute = 0;
        currentSecond = 0;
    }
}
