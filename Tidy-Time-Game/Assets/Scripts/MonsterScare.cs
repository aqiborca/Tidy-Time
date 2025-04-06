using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterScare : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    [Header("Monster References")]
    [SerializeField] private GameObject monsterObject; // This was missing - causing the error
    [SerializeField] private AudioSource scareSound;
    
    [Header("Animation Settings")]
    [SerializeField] private float startYPosition;
    [SerializeField] private float targetYPosition;
    [SerializeField] private float startScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float riseDuration;
    [SerializeField] private float holdDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float textDelay;
    [SerializeField] private float textDisplayDuration;

    private SceneSwitcher sceneSwitcher;
    private Vector3 originalScale;
    private SpriteRenderer monsterRenderer;

    private void Start()
    {
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        gameOverText.gameObject.SetActive(false);
        
        // Get references
        monsterRenderer = monsterObject.GetComponent<SpriteRenderer>();
        originalScale = monsterObject.transform.localScale;

        // Set initial position and scale
        monsterObject.transform.position = new Vector3(
            Camera.main.transform.position.x,
            startYPosition,
            monsterObject.transform.position.z
        );
        monsterObject.transform.localScale = originalScale * startScale;
        
        StartCoroutine(ScareSequence());
    }

    private IEnumerator ScareSequence()
    {
        yield return null;
        
        if (scareSound != null)
        {
            scareSound.Play();
        }

        // Rise up animation
        float riseElapsed = 0f;
        Vector3 startPosition = monsterObject.transform.position;
        Vector3 targetPosition = new Vector3(
            Camera.main.transform.position.x,
            targetYPosition,
            startPosition.z
        );

        while (riseElapsed < riseDuration)
        {
            float progress = riseElapsed / riseDuration;
            monsterObject.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                EaseOutQuad(progress)
            );
            
            monsterObject.transform.localScale = originalScale * Mathf.Lerp(
                startScale,
                maxScale,
                EaseOutQuad(progress)
            );
            
            riseElapsed += Time.deltaTime;
            yield return null;
        }

        // Hold at target position
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        float fadeElapsed = 0f;
        Color originalColor = monsterRenderer.color;
        
        while (fadeElapsed < fadeDuration)
        {
            float progress = fadeElapsed / fadeDuration;
            monsterRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                Mathf.Lerp(1f, 0f, progress)
            );
            fadeElapsed += Time.deltaTime;
            yield return null;
        }

        monsterObject.SetActive(false);

        // Show text after delay
        yield return new WaitForSeconds(textDelay);
        gameOverText.gameObject.SetActive(true);

        // Wait before scene change
        yield return new WaitForSeconds(textDisplayDuration);
        
        if (sceneSwitcher != null)
        {
            sceneSwitcher.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private float EaseOutQuad(float t) {
        return t * (2 - t);
    }
}