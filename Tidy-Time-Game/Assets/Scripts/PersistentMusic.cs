using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Destroy music in main menu (0), call mom scene (10), or jumpscare scene (12)
        if (sceneIndex == 0 || sceneIndex == 10 || sceneIndex == 12)
        {
            // Fade out music before destroying
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, Time.deltaTime);
                if (audioSource.volume <= 0.01f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}