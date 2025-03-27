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
        
        // Store references before making changes
        Transform originalParent = currentDraggedLetter.startParent;
        DraggableLetter existingLetter = transform.childCount > 0 ? 
            transform.GetChild(0).GetComponent<DraggableLetter>() : null;

        // Handle the swap
        if (existingLetter != null)
        {
            existingLetter.transform.SetParent(originalParent);
            existingLetter.rectTransform.anchoredPosition = currentDraggedLetter.startPosition;
            existingLetter.startParent = originalParent;
            existingLetter.startPosition = currentDraggedLetter.startPosition;
            existingLetter.isCorrectlyPlaced = false;
        }

        currentDraggedLetter.transform.SetParent(transform);
        currentDraggedLetter.rectTransform.anchoredPosition = Vector3.zero;
        currentDraggedLetter.startParent = transform;
        currentDraggedLetter.startPosition = Vector3.zero;

        // Check placement for BOTH letters involved in the swap
        currentDraggedLetter.CheckPlacement();
        if (existingLetter != null)
        {
            existingLetter.CheckPlacement();
        }
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