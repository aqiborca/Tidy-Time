using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject flashlightObject;
    public Button flashlightButton;
    private float fadeDuration = 0.1f;
    private float clickFlashDuration = 0.01f;

    private Renderer flashlightRenderer;
    private bool isFading = false;
    private Coroutine flashCoroutine;
    private bool isHolding = false;

    private void Start()
    {
        flashlightRenderer = flashlightObject.GetComponent<Renderer>();
        SetObjectAlpha(0f);

        var eventTrigger = flashlightButton.gameObject.AddComponent<EventTrigger>();

        // Pointer Down (hold to keep on)
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { OnButtonPressed(); });
        eventTrigger.triggers.Add(pointerDown);

        // Pointer Up (release to turn off)
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
        eventTrigger.triggers.Add(pointerUp);

        // Click (quick flash)
        flashlightButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonPressed()
    {
        isHolding = true;
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        if (!isFading)
            StartCoroutine(FadeObject(1f));

        if (MonsterSpawnManager.Instance != null && MonsterSpawnManager.Instance.IsMonsterActive())
        {
            MonsterSpawnManager.Instance.ScareAwayMonster();
            Debug.Log("[Flashlight] Monster scared away!");
        }
    }

    private void OnButtonReleased()
    {
        isHolding = false;
        if (!isFading && flashCoroutine == null)
            StartCoroutine(FadeObject(0f));
    }

    private void OnButtonClicked()
    {
        if (!isHolding) // Only trigger flash if not holding
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(QuickFlash());
        }
    }

    private IEnumerator QuickFlash()
    {
        // Fade in
        yield return StartCoroutine(FadeObject(1f));
        
        // Stay on for 1 second
        yield return new WaitForSeconds(clickFlashDuration);
        
        // Fade out (only if not being held)
        if (!isHolding)
            yield return StartCoroutine(FadeObject(0f));
        
        flashCoroutine = null;
    }

    private IEnumerator FadeObject(float targetAlpha)
    {
        isFading = true;
        float startAlpha = flashlightRenderer.material.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetObjectAlpha(newAlpha);
            yield return null;
        }

        SetObjectAlpha(targetAlpha);
        isFading = false;
    }

    private void SetObjectAlpha(float alpha)
    {
        Color color = flashlightRenderer.material.color;
        color.a = alpha;
        flashlightRenderer.material.color = color;
    }
}