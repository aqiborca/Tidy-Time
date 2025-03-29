using UnityEngine;
using UnityEngine.UI;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject completionPanel;
    public Button closeButton;

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

        // Initialize UI
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => {
                if (completionPanel != null)
                {
                    completionPanel.SetActive(false);
                }
            });
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