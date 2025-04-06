using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance;

    public bool monsterIsActive = false;
    public AudioClip monsterSpawnSound;
    private AudioSource audioSource;

    // Spawn timing variables
    private float initialSpawnCooldownMin = 30f;  // Starting min time between spawns
    private float initialSpawnCooldownMax = 60f;  // Starting max time between spawns
    private float finalSpawnCooldownMin = 10f;    // Final min time between spawns
    private float finalSpawnCooldownMax = 20f;    // Final max time between spawns
    private float monsterSpawnDuration = 20f;     // Time monster stays active before game over
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

    IEnumerator MonsterSpawnLoop()
    {
        yield return new WaitForSeconds(3f); // Wait for timer to initialize

        while (!gameOverTriggered)
        {
            // Calculate current spawn cooldown based on game progress (4PM to 9PM)
            float progress = Mathf.Clamp01((timerScript.GetCurrentHour() - 4 + timerScript.GetCurrentMinute() / 60f) / 5f);
            float currentMinCooldown = Mathf.Lerp(initialSpawnCooldownMin, finalSpawnCooldownMin, progress);
            float currentMaxCooldown = Mathf.Lerp(initialSpawnCooldownMax, finalSpawnCooldownMax, progress);

            float cooldown = Random.Range(currentMinCooldown, currentMaxCooldown);
            Debug.Log($"[Monster Manager] Current spawn interval: {currentMinCooldown:F1}-{currentMaxCooldown:F1}s " +
                     $"(Progress: {progress:P0}). Waiting {cooldown:F1} seconds.");
            
            yield return new WaitForSeconds(cooldown);

            StartCoroutine(SpawnMonsterRoutine());

            // Wait until the monster is gone before starting next loop
            while (monsterIsActive && !gameOverTriggered)
            {
                yield return null;
            }
        }
    }

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
            if (!monsterIsActive)
            {
                Debug.Log("[Monster Manager] Monster scared away!");
                yield break;
            }

            yield return new WaitForSeconds(1f);
            timeWaited += 1f;
        }

        if (monsterIsActive)
        {
            Debug.Log("[Monster Manager] Monster was not scared away in time! Game Over.");
            gameOverTriggered = true;
            TriggerGameOver();
        }
    }

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
        SceneManager.LoadScene("Call Mom");
    }

    public bool IsMonsterActive()
    {
        return monsterIsActive;
    }
}