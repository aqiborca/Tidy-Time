using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemName = "Item"; // for debugging
    public FishManager fishManager;  // Reference to FishManager

    void OnMouseDown()
    {
        Debug.Log(itemName + " collected");
        
        // Notify FishManager that a letter was collected
        if (fishManager != null)
        {
            fishManager.CollectLetter(gameObject); // Pass the collected letter
        }
        
        gameObject.SetActive(false); // Hide the collected object
    }
}
