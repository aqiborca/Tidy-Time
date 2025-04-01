using UnityEngine;

public class SoupManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject completionPanel;
    
    [Header("Chore Settings")]
    public string choreName = "AlphabetSoup";
    
    private LetterRandomizer letterRandomizer;

    private void Start()
    {
        letterRandomizer = FindObjectOfType<LetterRandomizer>();
        
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
        
        if (ChoreManager.Instance != null && ChoreManager.Instance.IsChoreCompleted(choreName))
        {
            PlaceLettersCorrectly();
            MarkChoreComplete();
        }
        else
        {
            letterRandomizer.RandomizeLetters(false);
        }

        // Hide uncollected letters (B, U, R, P)
        string[] collectedLetters = { "letterB", "letterU", "letterR", "letterP" };

        foreach (string letterID in collectedLetters)
        {
            DraggableLetter[] allLetters = FindObjectsOfType<DraggableLetter>();
            foreach (DraggableLetter letter in allLetters)
            {
                ItemsIDTracking idTracker = letter.GetComponent<ItemsIDTracking>();
                if (idTracker != null && idTracker.itemID == letterID)
                {
                    if (!ItemCollectionTracker.IsCollected(letterID))
                    {
                        // Hide the letter if it hasn't been collected yet
                        letter.gameObject.SetActive(false);
                        Debug.Log(letterID + " not collected — hiding");
                    }
                    else
                    {
                        Debug.Log(letterID + " was collected — showing");
                    }
                }
            }
        }
    }

    public void CheckForCompletion()
    {
        DraggableLetter[] allLetters = FindObjectsOfType<DraggableLetter>();
        foreach (DraggableLetter letter in allLetters)
        {
            if (!letter.isCorrectlyPlaced) return;
        }
        
        CompleteSoupChore();
    }

    private void CompleteSoupChore()
    {
        if (ChoreManager.Instance != null)
        {
            ChoreManager.Instance.CompleteChore(choreName);
        }
        MarkChoreComplete();
    }

    private void MarkChoreComplete()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
    }

    private void PlaceLettersCorrectly()
    {
        DraggableLetter[] allLetters = FindObjectsOfType<DraggableLetter>();
        LetterPlaceholder[] allPlaceholders = FindObjectsOfType<LetterPlaceholder>();
        
        foreach (DraggableLetter letter in allLetters)
        {
            string letterName = letter.name.Replace("Letter ", "");
            foreach (LetterPlaceholder placeholder in allPlaceholders)
            {
                if (placeholder.name == "Placeholder " + letterName)
                {
                    letter.transform.SetParent(placeholder.transform);
                    letter.rectTransform.anchoredPosition = Vector2.zero;
                    letter.startParent = placeholder.transform;
                    letter.startPosition = Vector2.zero;
                    letter.isCorrectlyPlaced = true;
                    break;
                }
            }
        }
    }
}