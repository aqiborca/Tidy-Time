using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance;

    public bool monsterIsActive = false;
    public AudioSource monsterAudioSource;
    public FlashlightToggle flashlightToggle;
    public AudioClip monsterApproachSound;

    // Spawn timing variables
    private float initialSpawnCooldownMin = 30f;
    private float initialSpawnCooldownMax = 60f;
    private float finalSpawnCooldownMin = 10f;
    private float finalSpawnCooldownMax = 20f;
    private float monsterSpawnDuration = 20f;
    private bool gameOverTriggered = false;
    private bool finalSequenceActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (monsterAudioSource != null)
            {
                DontDestroyOnLoad(monsterAudioSource.gameObject);
                monsterAudioSource.volume = 0.3f;
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
        flashlightToggle = FindObjectOfType<FlashlightToggle>();
        StartCoroutine(MonsterSpawnLoop());
    }

    IEnumerator MonsterSpawnLoop()
    {
        yield return new WaitForSeconds(3f);

        while (!gameOverTriggered && !finalSequenceActive)
        {
            float progress = GetTimeProgress();
            float currentMinCooldown = Mathf.Lerp(initialSpawnCooldownMin, finalSpawnCooldownMin, progress);
            float currentMaxCooldown = Mathf.Lerp(initialSpawnCooldownMax, finalSpawnCooldownMax, progress);

            float cooldown = Random.Range(currentMinCooldown, currentMaxCooldown);
            Debug.Log($"[Monster] Next spawn in {cooldown:F1} seconds");

            yield return new WaitForSeconds(cooldown);

            if (!finalSequenceActive)
            {
                yield return StartCoroutine(SpawnMonsterRoutine());
            }
        }
    }

    IEnumerator SpawnMonsterRoutine()
    {
        monsterIsActive = true;
        Debug.Log("[Monster] Monster has spawned!");

        if (monsterAudioSource != null)
        {
            monsterAudioSource.Play();
        }

        float timeWaited = 0f;
        while (timeWaited < monsterSpawnDuration && !finalSequenceActive)
        {
            if (!monsterIsActive)
            {
                Debug.Log("[Monster] Monster scared away!");
                if (monsterAudioSource != null && monsterAudioSource.isPlaying)
                {
                    monsterAudioSource.Stop();
                }
                yield break;
            }

            yield return new WaitForSeconds(1f);
            timeWaited += 1f;
        }

        if (monsterIsActive && !finalSequenceActive)
        {
            TriggerGameOver();
        }
    }

    public void TriggerFinalSequence()
    {
        finalSequenceActive = true;
        Debug.Log("[Monster] Starting final sequence!");

        // Disable flashlight
        if (flashlightToggle != null)
        {
            flashlightToggle.DisableFlashlight();
        }

        // Play monster approach sound
        if (monsterAudioSource != null && monsterApproachSound != null)
        {
            monsterAudioSource.clip = monsterApproachSound;
            monsterAudioSource.Play();
        }

        // Force spawn the monster
        StartCoroutine(ForceSpawnMonster());
    }

    IEnumerator ForceSpawnMonster()
    {
        monsterIsActive = true;
        Debug.Log("[Monster] Final monster spawn!");

        // Wait for 20 seconds before jumpscare
        yield return new WaitForSeconds(20f);

        TriggerGameOver();
    }

    public void ScareAwayMonster()
    {
        if (monsterIsActive && !finalSequenceActive)
        {
            monsterIsActive = false;
            if (monsterAudioSource != null && monsterAudioSource.isPlaying)
            {
                monsterAudioSource.Stop();
            }
        }
    }

    public void TriggerGameOver()
    {
        if (gameOverTriggered) return;

        gameOverTriggered = true;
        Debug.Log("[Monster] Triggering game over!");
        SceneManager.LoadScene(12); // Jumpscare scene
    }

    public bool IsMonsterActive() => monsterIsActive;

    private float GetTimeProgress()
    {
        // Safe way to get time progress without direct Instance access
        TimerScript timer = FindObjectOfType<TimerScript>();
        if (timer == null)
        {
            Debug.LogWarning("TimerScript not found in scene");
            return 0f;
        }

        return Mathf.Clamp01((timer.GetCurrentHour() - 4 + timer.GetCurrentMinute() / 60f) / 5f);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 10) // Call Mom scene
        {
            CleanUpForCallMom();
        }
    }

    void CleanUpForCallMom()
    {
        if (monsterAudioSource != null && monsterAudioSource.isPlaying)
        {
            monsterAudioSource.Stop();
        }

        StopAllCoroutines();

        if (monsterAudioSource != null)
        {
            Destroy(monsterAudioSource.gameObject);
        }

        Destroy(gameObject);
    }
}