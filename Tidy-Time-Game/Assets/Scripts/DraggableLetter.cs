using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableLetter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public CanvasGroup canvasGroup;
    public Vector2 startPosition;
    public Transform startParent;
    public bool isCorrectlyPlaced = false;
    
    [Header("Hover Settings")]
    [Range(0.1f, 1f)]
    public float hoverAlpha = 0.7f;
    private float originalAlpha;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        originalAlpha = canvasGroup.alpha;
    }

    private void Start()
    {
        // Initialize start position and parent
        startParent = transform.parent;
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCorrectlyPlaced)
        {
            canvasGroup.alpha = hoverAlpha;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = originalAlpha;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCorrectlyPlaced) return;
        
        startPosition = rectTransform.anchoredPosition;
        startParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = originalAlpha;
        
        LetterPlaceholder.OnDragStarted(this);
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCorrectlyPlaced) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        LetterPlaceholder.OnDragEnded();

        if (transform.parent == canvas.transform)
        {
            ReturnToStartPosition();
        }
    }

    public void ReturnToStartPosition()
    {
        transform.SetParent(startParent);
        rectTransform.anchoredPosition = startPosition;
        isCorrectlyPlaced = false;
    }
}