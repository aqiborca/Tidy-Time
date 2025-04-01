using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance;

    public bool monsterIsActive = false;
    public AudioClip monsterSpawnSound;
    private AudioSource audioSource;

    private float spawnCooldownMin = 30f;  // Min time between spawns
    private float spawnCooldownMax = 60f;  // Max time between spawns
    private float monsterSpawnDuration = 20f;  // Time monster stays active before game over
    private bool gameOverTriggered = false;

    private TimerScript timerScript;

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

    void Start()
    {
        timerScript = FindObjectOfType<TimerScript>();
        if (timerScript == null)
        {
            Debug.LogError("TimerScript not found.");
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(MonsterSpawnLoop());
    }

    // Loop with randomized time between spawns
    IEnumerator MonsterSpawnLoop()
    {
        yield return new WaitForSeconds(3f); // Wait for timer to initialize

        while (!gameOverTriggered)
        {
            float cooldown = Random.Range(spawnCooldownMin, spawnCooldownMax);
            Debug.Log($"[Monster Manager] Waiting {cooldown:F1} seconds before next monster spawn.");
            yield return new WaitForSeconds(cooldown);  //Wait a certain amount of time before spawning the monster

            StartCoroutine(SpawnMonsterRoutine());

            // Wait until the monster is gone before starting next loop
            while (monsterIsActive && !gameOverTriggered)
            {
                yield return null;
            }
        }
    }

    // Handle monster behaviour when it's active
    IEnumerator SpawnMonsterRoutine()
    {
        monsterIsActive = true;

        Debug.Log("[Monster Manager] Monster has spawned!");
        if (monsterSpawnSound != null)
        {
            audioSource.clip = monsterSpawnSound;
            audioSource.Play();
        }

        float timeWaited = 0f;
        while (timeWaited < monsterSpawnDuration)
        {
            // Stop if monster was scared away with flashlight
            if (!monsterIsActive)
            {
                Debug.Log("[Monster Manager] Monster scared away!");
                yield break;
            }

            yield return new WaitForSeconds(1f);
            timeWaited += 1f;
        }

        // Trigger game over if the monster was not scared away on time
        if (monsterIsActive)
        {
            Debug.Log("[Monster Manager] Monster was not scared away in time! Game Over.");
            gameOverTriggered = true;
            TriggerGameOver();
        }
    }

    // Called by flashlight to make the monster go away
    public void ScareAwayMonster()
    {
        if (monsterIsActive)
        {
            monsterIsActive = false;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void TriggerGameOver()
    {
        SceneManager.LoadScene("Call Mom"); // replace with game over scene later
    }

    // Used by other scripts to check if monster is currently active
    public bool IsMonsterActive()
    {
        return monsterIsActive;
    }
}
