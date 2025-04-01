using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // If the current scene's build index is 0 or 10, destroy the music object
        if (sceneIndex == 0 || sceneIndex == 10)
        {
            Destroy(gameObject);
        }
    }
}