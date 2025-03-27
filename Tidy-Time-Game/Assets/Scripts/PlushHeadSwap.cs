using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushHeadSwap : MonoBehaviour
{
    // Movement and state variables
    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging = false;
    private bool isLocked = false;
    private bool isHovered = false;
    
    // Visual components
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polyCollider;

    // Configuration
    public Transform correctBody;
    public float snapThreshold = 0.5f;
    public float lockThreshold = 0.3f;
    public float dragAlpha = 0.6f;
    public float hoverAlpha = 0.8f;
    public Vector3 headPositionOffset = new Vector3(0, 0.2f, 0); // Added vertical offset

    // Static references
    private static List<Transform> allBodies = new List<Transform>();
    private static Dictionary<Transform, PlushHeadSwap> bodyToHeadMap = new Dictionary<Transform, PlushHeadSwap>();
    private static List<PlushHeadSwap> allHeads = new List<PlushHeadSwap>();

    void Awake()
    {
        allHeads.Add(this);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        polyCollider = GetComponent<PolygonCollider2D>();
        
        InitializeBodies();
        AssignStartingPosition();
        CheckAndUpdateLockStatus(true);
    }

    void OnMouseEnter()
    {
        if (!isLocked && !isDragging)
        {
            isHovered = true;
            UpdateHoverState();
        }
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateHoverState();
    }

    private void UpdateHoverState()
    {
        if (isHovered && !isLocked && !isDragging)
        {
            spriteRenderer.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                hoverAlpha
            );
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void InitializeBodies()
    {
        if (allBodies.Count == 0)
        {
            foreach (PlushHeadSwap head in allHeads)
            {
                if (head.correctBody != null && !allBodies.Contains(head.correctBody))
                {
                    allBodies.Add(head.correctBody);
                }
            }
        }
    }

    private void AssignStartingPosition()
    {
        if (SaveManager.Instance != null)
        {
            Vector3 savedPosition = SaveManager.Instance.GetPlushiePosition(gameObject.name, Vector3.zero);
            if (savedPosition != Vector3.zero)
            {
                transform.position = savedPosition;
                startPosition = savedPosition;
                
                foreach (var body in allBodies)
                {
                    if (body != null && Vector3.Distance(savedPosition, body.position + headPositionOffset) < 0.1f)
                    {
                        if (!bodyToHeadMap.ContainsKey(body) || bodyToHeadMap[body] == this)
                        {
                            bodyToHeadMap[body] = this;
                        }
                        break;
                    }
                }
                return;
            }
        }

        AssignToAvailableBody();
    }

    private void AssignToAvailableBody()
    {
        List<Transform> possibleBodies = new List<Transform>();
        
        foreach (var body in allBodies)
        {
            if (body == null) continue;
            if (body == correctBody) continue;
            if (!bodyToHeadMap.ContainsKey(body))
            {
                possibleBodies.Add(body);
            }
        }

        if (possibleBodies.Count == 0)
        {
            foreach (var body in allBodies)
            {
                if (body != null && body != correctBody)
                {
                    if (bodyToHeadMap.TryGetValue(body, out PlushHeadSwap otherHead))
                    {
                        otherHead.AssignRandomPosition();
                    }
                    possibleBodies.Add(body);
                    break;
                }
            }
        }

        if (possibleBodies.Count == 0)
        {
            startPosition = allBodies[0].position + headPositionOffset + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            transform.position = startPosition;
            return;
        }

        int randomIndex = Random.Range(0, possibleBodies.Count);
        Transform selectedBody = possibleBodies[randomIndex];
        
        if (selectedBody != null)
        {
            transform.position = selectedBody.position + headPositionOffset;
            startPosition = selectedBody.position + headPositionOffset;
            bodyToHeadMap[selectedBody] = this;
        }

        isLocked = false;
    }

    private void OnMouseDown()
    {
        if (isLocked) return;

        isDragging = true;
        isHovered = false;
        UpdateHoverState();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        foreach (var pair in new Dictionary<Transform, PlushHeadSwap>(bodyToHeadMap))
        {
            if (pair.Value == this)
            {
                bodyToHeadMap.Remove(pair.Key);
                break;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging && !isLocked)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
            transform.position = mousePosition + dragOffset;

            CheckForHoveredHeads();
        }
    }

    private void CheckForHoveredHeads()
    {
        Vector2 currentPos = transform.position;
        
        foreach (PlushHeadSwap head in allHeads)
        {
            if (head != this && !head.isDragging && head.polyCollider != null)
            {
                if (head.polyCollider.OverlapPoint(currentPos))
                {
                    head.SetTransparency(dragAlpha);
                }
                else
                {
                    head.ResetTransparency();
                }
            }
            else
            {
                head.ResetTransparency();
            }
        }
    }

    private void OnMouseUp()
    {
        if (isLocked) return;
        
        ResetAllHeadsTransparency();
        isDragging = false;
        UpdateHoverState();

        PlushHeadSwap targetHead = GetHeadUnderMouse();
        if (targetHead != null)
        {
            SwapWithHead(targetHead);
            return;
        }

        Transform closestBody = GetClosestBody();
        if (closestBody != null)
        {
            HandleBodyDrop(closestBody);
            return;
        }

        ReturnToStartPosition();
    }

    private PlushHeadSwap GetHeadUnderMouse()
    {
        foreach (PlushHeadSwap head in allHeads)
        {
            if (head != this && !head.isDragging && head.polyCollider != null)
            {
                if (head.polyCollider.OverlapPoint(transform.position))
                {
                    return head;
                }
            }
        }
        return null;
    }

    private Transform GetClosestBody()
    {
        Transform closestBody = null;
        float closestDistance = Mathf.Infinity;

        foreach (var body in allBodies)
        {
            if (body == null) continue;

            float distance = Vector3.Distance(transform.position, body.position + headPositionOffset);
            if (distance < snapThreshold && distance < closestDistance)
            {
                closestDistance = distance;
                closestBody = body;
            }
        }
        return closestBody;
    }

    private void HandleBodyDrop(Transform body)
    {
        if (bodyToHeadMap.ContainsKey(body))
        {
            PlushHeadSwap otherHead = bodyToHeadMap[body];
            SwapWithHead(otherHead);
        }
        else
        {
            transform.position = body.position + headPositionOffset;
            startPosition = body.position + headPositionOffset;
            bodyToHeadMap[body] = this;

            if (body == correctBody)
            {
                CheckAndUpdateLockStatus();
                if (PlushGameManager.Instance != null)
                {
                    PlushGameManager.Instance.CheckTaskCompletion();
                }
            }

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
            }
        }
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPosition;
        
        foreach (var body in allBodies)
        {
            if (body != null && Vector3.Distance(startPosition, body.position + headPositionOffset) < 0.1f)
            {
                bodyToHeadMap[body] = this;
                break;
            }
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
        }
    }

    private void SwapWithHead(PlushHeadSwap otherHead)
    {
        if (otherHead == null) return;

        Vector3 tempPosition = startPosition;
        startPosition = otherHead.startPosition;
        otherHead.startPosition = tempPosition;

        transform.position = startPosition;
        otherHead.transform.position = otherHead.startPosition;

        UpdateBodyMapping(this);
        UpdateBodyMapping(otherHead);

        CheckAndUpdateLockStatus(true);
        otherHead.CheckAndUpdateLockStatus(true);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
            SaveManager.Instance.SavePlushiePosition(otherHead.gameObject.name, otherHead.transform.position);
        }

        if (PlushGameManager.Instance != null)
        {
            PlushGameManager.Instance.CheckTaskCompletion();
        }
    }

    private void UpdateBodyMapping(PlushHeadSwap head)
    {
        foreach (var pair in new Dictionary<Transform, PlushHeadSwap>(bodyToHeadMap))
        {
            if (pair.Value == head)
            {
                bodyToHeadMap.Remove(pair.Key);
                break;
            }
        }

        foreach (var body in allBodies)
        {
            if (body != null && Vector3.Distance(head.transform.position, body.position + headPositionOffset) < 0.1f)
            {
                bodyToHeadMap[body] = head;
                break;
            }
        }
    }

    private void ResetAllHeadsTransparency()
    {
        foreach (PlushHeadSwap head in allHeads)
        {
            head.ResetTransparency();
        }
    }

    public void SetTransparency(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color newColor = originalColor;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }
    }

    public void ResetTransparency()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void CheckAndUpdateLockStatus(bool forceCheck = false)
    {
        if (correctBody == null) return;

        float distance = Vector3.Distance(transform.position, correctBody.position + headPositionOffset);
        bool shouldBeLocked = distance < lockThreshold;

        if (shouldBeLocked)
        {
            transform.position = correctBody.position + headPositionOffset;
            startPosition = correctBody.position + headPositionOffset;
            isLocked = true;
            
            if (!bodyToHeadMap.ContainsKey(correctBody) || bodyToHeadMap[correctBody] != this)
            {
                UpdateBodyMapping(this);
            }
        }
        else if (forceCheck || isLocked)
        {
            isLocked = false;
        }
    }

    public bool IsCorrectlyPlaced()
    {
        return isLocked;
    }

    void OnDestroy()
    {
        allHeads.Remove(this);
    }

    private void AssignRandomPosition()
    {
        Transform currentBody = null;
        foreach (var pair in bodyToHeadMap)
        {
            if (pair.Value == this)
            {
                currentBody = pair.Key;
                break;
            }
        }

        if (currentBody != null)
        {
            bodyToHeadMap.Remove(currentBody);
        }

        AssignToAvailableBody();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
        }
    }
}