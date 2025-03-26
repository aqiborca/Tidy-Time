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

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        polyCollider = GetComponent<PolygonCollider2D>();

        if (SaveManager.Instance != null)
        {
            Vector3 savedPosition = SaveManager.Instance.GetPlushiePosition(gameObject.name, startPosition);
            transform.position = savedPosition;
            startPosition = savedPosition;
        }
        
        CheckAndUpdateLockStatus();
    }

    private void OnMouseDown()
    {
        if (isLocked) return;

        isDragging = true;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = transform.position.z;
        dragOffset = transform.position - mouseWorldPos;
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
                // Convert position to the head's local space
                Vector2 point = head.transform.InverseTransformPoint(currentPos);
                
                // Precise point-in-polygon check
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

        // Precise collision detection using poly collider
        PlushHeadSwap targetHead = null;
        Collider2D targetBody = null;

        // Check for heads first
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

        // If no head found, check for bodies
        if (targetHead == null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, snapThreshold);
            foreach (Collider2D col in hitColliders)
            {
                if (col.CompareTag("PlushBody"))
                {
                    targetBody = col;
                    break;
                }
            }
        }

        if (targetHead != null)
        {
            SwapHeads(targetHead);
        }
        else if (targetBody != null)
        {
            if (targetBody.transform == correctBody)
            {
                transform.position = correctBody.position;
                startPosition = correctBody.position;
            }
            else
            {
                transform.position = targetBody.transform.position;
                startPosition = targetBody.transform.position;
            }
        }
        else
        {
            transform.position = startPosition;
        }

        CheckAndUpdateLockStatus();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
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

    private void SwapHeads(PlushHeadSwap otherHead)
    {
        if (otherHead != null && otherHead != this)
        {
            Vector3 tempPosition = startPosition;
            startPosition = otherHead.startPosition;
            otherHead.startPosition = tempPosition;

            transform.position = startPosition;
            otherHead.transform.position = otherHead.startPosition;

            CheckAndUpdateLockStatus();
            otherHead.CheckAndUpdateLockStatus();

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
                SaveManager.Instance.SavePlushiePosition(otherHead.gameObject.name, otherHead.transform.position);
            }
        }

        PlushGameManager.Instance.CheckTaskCompletion();
    }

    private void CheckAndUpdateLockStatus()
    {
        float distance = Vector3.Distance(transform.position, correctBody.position);
        bool shouldBeLocked = distance < lockThreshold;

        if (shouldBeLocked && !isLocked)
        {
            transform.position = correctBody.position;
            startPosition = correctBody.position;
            isLocked = true;
        }
        else if (!shouldBeLocked && isLocked)
        {
            isLocked = false;
        }
    }

    public bool IsCorrectlyPlaced()
    {
        return isLocked;
    }
}