using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushHeadSwap : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging = false;
    private bool isLocked = false;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polyCollider;

    public Transform correctBody;
    public float snapThreshold = 0.5f;
    public float lockThreshold = 0.3f;
    public float hoverAlpha = 0.6f;
    public float hoverBeforePickupAlpha = 0.8f;

    private List<Transform> availableBodies = new List<Transform>();
    private Dictionary<Transform, PlushHeadSwap> bodyToHeadMap = new Dictionary<Transform, PlushHeadSwap>();
    private static List<Transform> occupiedBodies = new List<Transform>();
    private static bool positionsAssigned = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        polyCollider = GetComponent<PolygonCollider2D>();

        InitializeBodies();

        if (!positionsAssigned)
        {
            occupiedBodies.Clear();
            positionsAssigned = true;
        }
        
        if (SaveManager.Instance != null)
        {
            Vector3 savedPosition = SaveManager.Instance.GetPlushiePosition(gameObject.name, Vector3.zero);
            if (savedPosition != Vector3.zero)
            {
                transform.position = savedPosition;
                startPosition = savedPosition;
                
                foreach (var body in availableBodies)
                {
                    if (body != null && Vector3.Distance(savedPosition, body.position) < 0.1f)
                    {
                        if (!occupiedBodies.Contains(body))
                        {
                            occupiedBodies.Add(body);
                            bodyToHeadMap[body] = this;
                        }
                        break;
                    }
                }
            }
            else
            {
                AssignRandomPosition();
            }
        }
        else
        {
            AssignRandomPosition();
        }
        
        CheckAndUpdateLockStatus(true);
    }

    private void InitializeBodies()
    {
        availableBodies.Clear();
        bodyToHeadMap.Clear();

        PlushHeadSwap[] allHeads = FindObjectsOfType<PlushHeadSwap>();
        foreach (PlushHeadSwap head in allHeads)
        {
            if (head.correctBody != null && !availableBodies.Contains(head.correctBody))
            {
                availableBodies.Add(head.correctBody);
            }
        }
    }

    private void AssignRandomPosition()
    {
        if (availableBodies.Count == 0) return;

        // Create a list of all possible bodies (excluding correct body)
        List<Transform> possibleBodies = new List<Transform>();
        foreach (var body in availableBodies)
        {
            if (body == null) continue;
            if (body == correctBody) continue;
            possibleBodies.Add(body);
        }

        // Remove already occupied bodies
        List<Transform> unoccupiedBodies = new List<Transform>();
        foreach (var body in possibleBodies)
        {
            if (!occupiedBodies.Contains(body))
            {
                unoccupiedBodies.Add(body);
            }
        }

        // If no unoccupied bodies left (shouldn't happen with proper setup)
        if (unoccupiedBodies.Count == 0)
        {
            // Place near a random body
            Transform fallbackBody = availableBodies[Random.Range(0, availableBodies.Count)];
            startPosition = fallbackBody.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            transform.position = startPosition;
            return;
        }

        // Select a random unoccupied body
        int randomIndex = Random.Range(0, unoccupiedBodies.Count);
        Transform selectedBody = unoccupiedBodies[randomIndex];
        
        if (selectedBody != null)
        {
            transform.position = selectedBody.position;
            startPosition = selectedBody.position;
            
            occupiedBodies.Add(selectedBody);
            bodyToHeadMap[selectedBody] = this;
        }

        isLocked = false;
    }

    private void OnMouseEnter()
    {
        if (!isDragging && !isLocked)
        {
            SetTransparency(hoverBeforePickupAlpha);
        }
    }

    private void OnMouseExit()
    {
        if (!isDragging && !isLocked)
        {
            ResetTransparency();
        }
    }

    private void OnMouseDown()
    {
        if (isLocked) return;

        ResetTransparency();
        isDragging = true;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        foreach (var pair in new Dictionary<Transform, PlushHeadSwap>(bodyToHeadMap))
        {
            if (pair.Value == this)
            {
                bodyToHeadMap.Remove(pair.Key);
                if (occupiedBodies.Contains(pair.Key))
                {
                    occupiedBodies.Remove(pair.Key);
                }
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
        PlushHeadSwap[] allHeads = FindObjectsOfType<PlushHeadSwap>();
        Vector2 currentPos = transform.position;
        
        foreach (PlushHeadSwap head in allHeads)
        {
            if (head != this && !head.isDragging && head.polyCollider != null)
            {
                if (head.polyCollider.OverlapPoint(currentPos))
                {
                    head.SetTransparency(hoverAlpha);
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

        if (correctBody == null)
        {
            transform.position = startPosition;
            return;
        }

        float distanceToCorrect = Vector3.Distance(transform.position, correctBody.position);
        if (distanceToCorrect < snapThreshold)
        {
            if (bodyToHeadMap.ContainsKey(correctBody))
            {
                PlushHeadSwap otherHead = bodyToHeadMap[correctBody];
                SwapWithHead(otherHead);
            }
            else
            {
                transform.position = startPosition;
                ReassignToPreviousBody();
            }
            return;
        }

        PlushHeadSwap targetHead = null;
        PlushHeadSwap[] allHeads = FindObjectsOfType<PlushHeadSwap>();
        
        foreach (PlushHeadSwap head in allHeads)
        {
            if (head != this && !head.isDragging && head.polyCollider != null)
            {
                if (head.polyCollider.OverlapPoint(transform.position))
                {
                    targetHead = head;
                    break;
                }
            }
        }

        if (targetHead != null)
        {
            SwapWithHead(targetHead);
        }
        else
        {
            transform.position = startPosition;
            ReassignToPreviousBody();
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
        }
    }

    private void ReassignToPreviousBody()
    {
        foreach (var body in availableBodies)
        {
            if (body != null && Vector3.Distance(startPosition, body.position) < 0.1f)
            {
                bodyToHeadMap[body] = this;
                if (!occupiedBodies.Contains(body))
                {
                    occupiedBodies.Add(body);
                }
                break;
            }
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
                if (occupiedBodies.Contains(pair.Key))
                {
                    occupiedBodies.Remove(pair.Key);
                }
                break;
            }
        }

        foreach (var body in availableBodies)
        {
            if (body != null && Vector3.Distance(head.transform.position, body.position) < 0.1f)
            {
                bodyToHeadMap[body] = head;
                if (!occupiedBodies.Contains(body))
                {
                    occupiedBodies.Add(body);
                }
                break;
            }
        }
    }

    private void ResetAllHeadsTransparency()
    {
        PlushHeadSwap[] allHeads = FindObjectsOfType<PlushHeadSwap>();
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

        float distance = Vector3.Distance(transform.position, correctBody.position);
        bool shouldBeLocked = distance < lockThreshold;

        if (shouldBeLocked)
        {
            transform.position = correctBody.position;
            startPosition = correctBody.position;
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

    private void OnDestroy()
    {
        positionsAssigned = false;
    }
}