using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableLetter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;

    // You can add a variable for the correct placeholder here if needed
    public bool isCorrectlyPlaced = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is missing on " + gameObject.name + ". Please add it.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // When dragging ends, check if it's correctly placed
        if (isCorrectlyPlaced)
        {
            OnCorrectPlacement();
        }
        else
        {
            OnIncorrectPlacement();
        }
    }

    // Make these methods public so they can be accessed from other scripts
    public void OnCorrectPlacement()
    {
        Debug.Log("Letter placed correctly!");
        // Call additional code to mark the chore as completed if needed
    }

    public void OnIncorrectPlacement()
    {
        rectTransform.anchoredPosition = startPosition;  // Snap back to the original position
        Debug.Log("Letter placed incorrectly!");
    }
}
