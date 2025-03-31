using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemID = "Item"; // Item tracking
    public FishManager fishManager;  // Reference to FishManager (for letters?)

    void OnMouseDown()
    {
        Debug.Log(itemID + " collected");

        ItemCollectionTracker.MarkCollected(itemID);

        // Notify FishManager that a letter was collected
        if (fishManager != null)
        {
            fishManager.CollectLetter(gameObject); // Pass the collected letter
        }
        
        gameObject.SetActive(false); // Hide the collected object
    }
}
