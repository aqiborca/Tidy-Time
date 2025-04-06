using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance;

    public bool monsterIsActive = false;
    public AudioSource monsterAudioSource; // Changed from AudioClip to AudioSource

    // Spawn timing variables
    private float initialSpawnCooldownMin = 30f;
    private float initialSpawnCooldownMax = 60f;
    private float finalSpawnCooldownMin = 10f;
    private float finalSpawnCooldownMax = 20f;
    private float monsterSpawnDuration = 20f;
    private bool gameOverTriggered = false;

    private TimerScript timerScript;
    private SceneSwitcher sceneSwitcher;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Ensure the AudioSource persists and is configured properly
            if (monsterAudioSource != null)
            {
                DontDestroyOnLoad(monsterAudioSource.gameObject);
                monsterAudioSource.volume = 0.3f; // Set volume to 0.3
                monsterAudioSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timerScript = FindObjectOfType<TimerScript>();
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        
        if (timerScript == null)
        {
            Debug.LogError("TimerScript not found.");
            return;
        }

        StartCoroutine(MonsterSpawnLoop());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reconnect references if needed when scene changes
        timerScript = FindObjectOfType<TimerScript>();
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
    }

    IEnumerator MonsterSpawnLoop()
    {
        yield return new WaitForSeconds(3f);

        while (!gameOverTriggered)
        {
            float progress = Mathf.Clamp01((timerScript.GetCurrentHour() - 4 + timerScript.GetCurrentMinute() / 60f) / 5f);
            float currentMinCooldown = Mathf.Lerp(initialSpawnCooldownMin, finalSpawnCooldownMin, progress);
            float currentMaxCooldown = Mathf.Lerp(initialSpawnCooldownMax, finalSpawnCooldownMax, progress);

            float cooldown = Random.Range(currentMinCooldown, currentMaxCooldown);
            Debug.Log($"[Monster Manager] Current spawn interval: {currentMinCooldown:F1}-{currentMaxCooldown:F1}s " +
                     $"(Progress: {progress:P0}). Waiting {cooldown:F1} seconds.");
            
            yield return new WaitForSeconds(cooldown);

            StartCoroutine(SpawnMonsterRoutine());

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
        if (monsterAudioSource != null)
        {
            monsterAudioSource.Play();
        }

        float timeWaited = 0f;
        while (timeWaited < monsterSpawnDuration)
        {
            if (!monsterIsActive)
            {
                Debug.Log("[Monster Manager] Monster scared away!");
                if (monsterAudioSource != null && monsterAudioSource.isPlaying)
                {
                    monsterAudioSource.Stop();
                }
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
            if (monsterAudioSource != null && monsterAudioSource.isPlaying)
            {
                monsterAudioSource.Stop();
            }
        }
    }

    void TriggerGameOver()
    {
        if (sceneSwitcher != null)
        {
            sceneSwitcher.LoadMonsterJumpscare();
        }
        else
        {
            Debug.LogWarning("SceneSwitcher not found, loading jumpscare directly");
            SceneManager.LoadScene(12); // Fallback to direct scene load
        }
    }

    public bool IsMonsterActive()
    {
        return monsterIsActive;
    }
}