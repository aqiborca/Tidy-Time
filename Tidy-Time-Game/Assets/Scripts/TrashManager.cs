using UnityEngine;
using UnityEngine.UI;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject completionPanel;

    [Header("Effects")]
    public ParticleSystem confettiEffect;
    public AudioClip successSound;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (ChoreManager.Instance != null)
        {
            Debug.Log("Checking garbage completion on scene load...");
            if (!ChoreManager.Instance.IsChoreCompleted("garbage"))
            {
                completionPanel.SetActive(false);
                Debug.Log("NOT COMPLETE");
            }
            else
            {
                completionPanel.SetActive(true);
                Debug.Log("IS COMPLETE");

                // Make all objects with "Trash" tag invisible
                GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash");
                foreach (GameObject trash in trashObjects)
                {
                    trash.SetActive(false); // Disables the object, making it invisible
                }
            }
        }
        else
        {
            Debug.LogError("ChoreManager instance is null in TrashManager!");
        }

        // Enable trash pieces that have been collected from other scenes
        GameObject[] allTrash = GameObject.FindGameObjectsWithTag("Trash");

        foreach (GameObject trash in allTrash)
        {
            ItemsIDTracking idTracker = trash.GetComponent<ItemsIDTracking>();
            if (idTracker != null)
            {
                if (ItemCollectionTracker.IsCollected(idTracker.itemID))
                {
                    trash.SetActive(true);
                    Debug.Log(idTracker.itemID + " was collected — showing trash piece.");
                }
                else
                {
                    trash.SetActive(false);
                    Debug.Log(idTracker.itemID + " not collected — hiding trash piece");
                }
            }
        }
    }

    public void NotifyTrashCompleted()
    {
        // Show completion UI
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }

        // Play celebration effects
        PlayCompletionEffects();
    }

    private void PlayCompletionEffects()
    {
        // Visual effect
        if (confettiEffect != null)
        {
            Instantiate(confettiEffect, Vector3.up * 2f, Quaternion.identity);
        }

        // Sound effect
        if (successSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);
        }
    }
}