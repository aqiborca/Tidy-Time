using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Image))]
public class LetterPlaceholder : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static bool isAnyDragActive;
    private static DraggableLetter currentDraggedLetter;
    private CanvasGroup existingLetterCanvasGroup;

    private void Awake()
    {
        GetComponent<Image>().enabled = false;
    }

    private void Start()
    {
        // Randomize positions every time the scene loads
        RandomizeLetterPositions();
    }

    private void RandomizeLetterPositions()
    {
        // Only randomize if this is a placeholder (not during gameplay)
        if (transform.childCount == 0)
        {
            var allLetters = FindObjectsOfType<DraggableLetter>()
                .Where(l => l.transform.parent.GetComponent<LetterPlaceholder>() == null)
                .OrderBy(x => Random.value)
                .ToList();

            if (allLetters.Count > 0)
            {
                // Take the first available letter
                var letter = allLetters.First();
                letter.transform.SetParent(transform);
                letter.rectTransform.anchoredPosition = Vector2.zero;
                letter.startParent = transform;
                letter.startPosition = Vector2.zero;
            }
        }
    }

    public static void OnDragStarted(DraggableLetter letter)
    {
        isAnyDragActive = true;
        currentDraggedLetter = letter;
    }

    public static void OnDragEnded()
    {
        if (currentDraggedLetter != null)
        {
            currentDraggedLetter.canvasGroup.alpha = 1f;
        }
        
        isAnyDragActive = false;
        currentDraggedLetter = null;
        
        foreach (var placeholder in FindObjectsOfType<LetterPlaceholder>())
        {
            if (placeholder.existingLetterCanvasGroup != null)
            {
                placeholder.existingLetterCanvasGroup.alpha = 1f;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!isAnyDragActive || currentDraggedLetter == null) return;
        
        if (existingLetterCanvasGroup != null)
        {
            existingLetterCanvasGroup.alpha = 1f;
        }

        if (transform.childCount > 0)
        {
            DraggableLetter existingLetter = transform.GetChild(0).GetComponent<DraggableLetter>();
            
            existingLetter.transform.SetParent(currentDraggedLetter.startParent);
            existingLetter.rectTransform.anchoredPosition = currentDraggedLetter.startPosition;
            existingLetter.startParent = currentDraggedLetter.startParent;
            existingLetter.startPosition = currentDraggedLetter.startPosition;
            existingLetter.isCorrectlyPlaced = false;
        }

        currentDraggedLetter.transform.SetParent(transform);
        currentDraggedLetter.rectTransform.anchoredPosition = Vector3.zero;
        currentDraggedLetter.startParent = transform;
        currentDraggedLetter.startPosition = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAnyDragActive && transform.childCount > 0)
        {
            existingLetterCanvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
            if (existingLetterCanvasGroup != null)
            {
                existingLetterCanvasGroup.alpha = 0.5f;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (existingLetterCanvasGroup != null)
        {
            existingLetterCanvasGroup.alpha = 1f;
            existingLetterCanvasGroup = null;
        }
    }
}