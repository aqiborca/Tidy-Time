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

    // Non-static references that get rebuilt each scene load
    private List<Transform> availableBodies = new List<Transform>();
    private Dictionary<Transform, PlushHeadSwap> bodyToHeadMap = new Dictionary<Transform, PlushHeadSwap>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        polyCollider = GetComponent<PolygonCollider2D>();

        // Rebuild references each time the scene loads
        InitializeBodies();
        
        // Load saved position or assign random position
        if (SaveManager.Instance != null)
        {
            Vector3 savedPosition = SaveManager.Instance.GetPlushiePosition(gameObject.name, Vector3.zero);
            if (savedPosition != Vector3.zero)
            {
                transform.position = savedPosition;
                startPosition = savedPosition;
                
                // Find which body this position corresponds to
                foreach (var body in availableBodies)
                {
                    if (body != null && Vector3.Distance(savedPosition, body.position) < 0.1f)
                    {
                        bodyToHeadMap[body] = this;
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

        // Create list of possible bodies (excluding correct body and already occupied bodies)
        List<Transform> possibleBodies = new List<Transform>();
        foreach (var body in availableBodies)
        {
            if (body == null) continue;
            
            // Don't allow heads to start on their correct body
            if (body == correctBody) continue;
            
            // Don't use bodies that already have a head
            if (!bodyToHeadMap.ContainsKey(body))
            {
                possibleBodies.Add(body);
            }
        }

        // If no valid bodies left (shouldn't happen with proper setup), use any non-correct body
        if (possibleBodies.Count == 0)
        {
            foreach (var body in availableBodies)
            {
                if (body != null && body != correctBody)
                {
                    possibleBodies.Add(body);
                    break;
                }
            }
        }

        // If still no bodies (only one head?), use a random position near bodies
        if (possibleBodies.Count == 0)
        {
            startPosition = availableBodies[0].position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            transform.position = startPosition;
            return;
        }

        // Randomly select one of the available bodies
        int randomIndex = Random.Range(0, possibleBodies.Count);
        Transform selectedBody = possibleBodies[randomIndex];
        
        if (selectedBody != null)
        {
            transform.position = selectedBody.position;
            startPosition = selectedBody.position;
            bodyToHeadMap[selectedBody] = this;
        }

        // We should never start locked since we excluded correct body
        isLocked = false;
    }

    private void OnMouseDown()
    {
        if (isLocked) return;

        isDragging = true;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;

        // Remove from body mapping when we start dragging
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

        // Null check for correctBody
        if (correctBody == null)
        {
            transform.position = startPosition;
            return;
        }

        // Check for correct body first
        float distanceToCorrect = Vector3.Distance(transform.position, correctBody.position);
        if (distanceToCorrect < snapThreshold)
        {
            // If correct body is already occupied, swap with that head
            if (bodyToHeadMap.ContainsKey(correctBody))
            {
                PlushHeadSwap otherHead = bodyToHeadMap[correctBody];
                SwapWithHead(otherHead);
                return;
            }

            // Otherwise take this position
            transform.position = correctBody.position;
            startPosition = correctBody.position;
            bodyToHeadMap[correctBody] = this;
            CheckAndUpdateLockStatus();
            
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
            }
            
            if (PlushGameManager.Instance != null)
            {
                PlushGameManager.Instance.CheckTaskCompletion();
            }
            return;
        }

        // Check for other bodies
        foreach (var body in availableBodies)
        {
            if (body == null) continue;
            
            if (body != correctBody && Vector3.Distance(transform.position, body.position) < snapThreshold)
            {
                // If body is occupied, swap with that head
                if (bodyToHeadMap.ContainsKey(body))
                {
                    PlushHeadSwap otherHead = bodyToHeadMap[body];
                    SwapWithHead(otherHead);
                    return;
                }

                // Otherwise take this position
                transform.position = body.position;
                startPosition = body.position;
                bodyToHeadMap[body] = this;
                
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
                }
                return;
            }
        }

        // Check for other heads (direct collision)
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
            // Return to start position if no valid placement
            transform.position = startPosition;
            
            // Try to find which body this was on before
            foreach (var body in availableBodies)
            {
                if (body != null && Vector3.Distance(startPosition, body.position) < 0.1f)
                {
                    bodyToHeadMap[body] = this;
                    break;
                }
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

        // Swap positions
        Vector3 tempPosition = startPosition;
        startPosition = otherHead.startPosition;
        otherHead.startPosition = tempPosition;

        transform.position = startPosition;
        otherHead.transform.position = otherHead.startPosition;

        // Update body mappings
        UpdateBodyMapping(this);
        UpdateBodyMapping(otherHead);

        // Force both heads to check their lock status
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
        // Remove old mapping
        foreach (var pair in new Dictionary<Transform, PlushHeadSwap>(bodyToHeadMap))
        {
            if (pair.Value == head)
            {
                bodyToHeadMap.Remove(pair.Key);
                break;
            }
        }

        // Add new mapping if on a body
        foreach (var body in availableBodies)
        {
            if (body != null && Vector3.Distance(head.transform.position, body.position) < 0.1f)
            {
                bodyToHeadMap[body] = head;
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
            // If we're close enough to the correct body, snap to it exactly
            transform.position = correctBody.position;
            startPosition = correctBody.position;
            isLocked = true;
            
            // Ensure we're in the body mapping
            if (!bodyToHeadMap.ContainsKey(correctBody) || bodyToHeadMap[correctBody] != this)
            {
                UpdateBodyMapping(this);
            }
        }
        else if (forceCheck || isLocked)
        {
            // Only unlock if we're forcing a check or if we were previously locked
            isLocked = false;
        }
    }

    public bool IsCorrectlyPlaced()
    {
        return isLocked;
    }
}