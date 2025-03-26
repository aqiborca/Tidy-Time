using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LetterPlaceholder : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static bool isAnyDragActive;
    private static DraggableLetter currentDraggedLetter;
    private CanvasGroup existingLetterCanvasGroup;

    private void Awake()
    {
        // Disable placeholder image visuals
        GetComponent<Image>().enabled = false;
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
        
        // Restore any faded letters
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
        
        // Reset transparency of existing letter
        if (existingLetterCanvasGroup != null)
        {
            existingLetterCanvasGroup.alpha = 1f;
        }

        // If this placeholder has a letter, swap them
        if (transform.childCount > 0)
        {
            DraggableLetter existingLetter = transform.GetChild(0).GetComponent<DraggableLetter>();
            
            existingLetter.transform.SetParent(currentDraggedLetter.startParent);
            existingLetter.rectTransform.anchoredPosition = currentDraggedLetter.startPosition;
            existingLetter.startParent = currentDraggedLetter.startParent;
            existingLetter.startPosition = currentDraggedLetter.startPosition;
            existingLetter.isCorrectlyPlaced = false;
        }

        // Place dragged letter in this placeholder
        currentDraggedLetter.transform.SetParent(transform);
        currentDraggedLetter.rectTransform.anchoredPosition = Vector3.zero;
        currentDraggedLetter.startParent = transform;
        currentDraggedLetter.startPosition = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make existing letter transparent when hovered during drag
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
        // Restore existing letter's opacity
        if (existingLetterCanvasGroup != null)
        {
            existingLetterCanvasGroup.alpha = 1f;
            existingLetterCanvasGroup = null;
        }
    }
}