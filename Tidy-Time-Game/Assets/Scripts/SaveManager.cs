using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    //dictionary to store plushie head positions
    private Dictionary<string, Vector3> plushiePositions = new Dictionary<string, Vector3>();

    //flag to mark that the fish feeding task has been completed
    public bool fishFeedingCompleted = false;

    //dictionary to store whether each letter has been collected
    private Dictionary<string, bool> letterCollected = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //methods for saving plush head positions
    public void SavePlushiePosition(string plushieID, Vector3 position)
    {
        if (plushiePositions.ContainsKey(plushieID))
            plushiePositions[plushieID] = position;
        else
            plushiePositions.Add(plushieID, position);
    }

    public Vector3 GetPlushiePosition(string plushieID, Vector3 defaultPosition)
    {
        if (plushiePositions.ContainsKey(plushieID))
            return plushiePositions[plushieID];
        return defaultPosition;
    }

    //methods for saving letter collection state
    public void SetLetterCollected(string letterID, bool collected)
    {
        if (letterCollected.ContainsKey(letterID))
            letterCollected[letterID] = collected;
        else
            letterCollected.Add(letterID, collected);
    }

    public bool GetLetterCollected(string letterID)
    {
        if (letterCollected.ContainsKey(letterID))
            return letterCollected[letterID];
        return false; 
    }
}
