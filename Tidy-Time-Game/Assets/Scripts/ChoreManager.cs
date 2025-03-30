using UnityEngine;

public class ChoreManager : MonoBehaviour
{
    // Boolean flags to track completion of each chore
    public bool isAlphabetSoupCompleted = false;
    public bool isSwapPlushiesCompleted = false;
    public bool isMathHomeworkCompleted = false;
    public bool isGarbageCompleted = false;
    public bool isOrganizeClosetCompleted = false;
    public bool isFeedFishCompleted = false;

    // Static instance for easier access
    public static ChoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    public bool IsChoreCompleted(string choreName)
    {
        switch (choreName.ToLower()) // Case-insensitive check
        {
            case "alphabetsoup":
                return isAlphabetSoupCompleted;
            case "swapplushies":
                return isSwapPlushiesCompleted;
            case "mathhomework":
                return isMathHomeworkCompleted;
            case "garbage":
                return isGarbageCompleted;
            case "organizecloset":
                return isOrganizeClosetCompleted;
            case "feedfish":
                return isFeedFishCompleted;
            default:
                Debug.LogWarning($"Unknown chore: {choreName}");
                return false;
        }
    }

    public void CompleteChore(string choreName)
    {
        switch (choreName.ToLower())
        {
            case "alphabetsoup":
                isAlphabetSoupCompleted = true;
                break;
            case "swapplushies":
                isSwapPlushiesCompleted = true;
                break;
            case "mathhomework":
                isMathHomeworkCompleted = true;
                break;
            case "garbage":
                isGarbageCompleted = true;
                break;
            case "organizecloset":
                isOrganizeClosetCompleted = true;
                break;
            case "feedfish":
                isFeedFishCompleted = true;
                break;
            default:
                Debug.LogWarning($"Unknown chore: {choreName}");
                return;
        }

        Debug.Log($"{choreName} completed!");
        CheckForGameCompletion();
    }

    private void CheckForGameCompletion()
    {
        if (AreAllChoresCompleted())
        {
            EndGame();
        }
    }

    private bool AreAllChoresCompleted()
    {
        return isAlphabetSoupCompleted &&
               isSwapPlushiesCompleted &&
               isMathHomeworkCompleted &&
               isGarbageCompleted &&
               isOrganizeClosetCompleted &&
               isFeedFishCompleted;;
    }

    private void EndGame()
    {
        Debug.Log("All chores completed! Game over.");
    }
}