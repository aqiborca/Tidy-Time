/*
This script allows for saving data between scenes
*/
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Variables
    public static DataManager Instance { get; private set; }
    public Vector3 playerPosition;

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
}