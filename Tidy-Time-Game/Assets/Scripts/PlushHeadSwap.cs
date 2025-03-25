//Note to self: TO DO -   figure out a way to implement detection of the right placement properly, work in progress.


using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlushHeadSwap : MonoBehaviour
{
    //stores the head�s last valid (snapped) position
    private Vector3 startPosition;
    //holds the offset between the head's position and the mouse click point
    private Vector3 dragOffset;
    //flag for whether this head is currently being dragged
    private bool isDragging = false;

    public Transform correctBody;

    void Start()
    {
        //store initial position as the valid body attachment position
        startPosition = transform.position;

        //retrieve saved position if it exists
        if (SaveManager.Instance != null)
        {
            Vector3 savedPosition = SaveManager.Instance.GetPlushiePosition(gameObject.name, startPosition);
            transform.position = savedPosition;
            startPosition = savedPosition;
        }
    }

    private void OnMouseDown()
    {
        UnityEngine.Debug.Log(gameObject.name + " clicked");  //had some issues with click detection - fixed now, leaving debug log just in case
        isDragging = true;
        //calculate the offset between the head position and the mouse's position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //keep Z position at 0
        mouseWorldPos.z = 0;
        dragOffset = transform.position - mouseWorldPos;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //get the current mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            //apply offset so the head doesn't "jump"
            transform.position = mousePosition + dragOffset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        //get the head's collider to use its bounds
        Collider2D myCollider = GetComponent<Collider2D>();
        //use OverlapBoxAll with collider's center and size
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(myCollider.bounds.center, myCollider.bounds.size, 0f);

        PlushHeadSwap targetHead = null;
        Collider2D targetBody = null;

        //look through all overlapping colliders
        foreach (Collider2D col in hitColliders)
        {
            //if another head is found, mark it as the target.
            if (col.CompareTag("PlushHead") && col.gameObject != gameObject)
            {
                targetHead = col.GetComponent<PlushHeadSwap>();
                break; //give swapping priority
            }
            //if a body is found, record it
            else if (col.CompareTag("PlushBody"))
            {
                targetBody = col;
            }
        }

        if (targetHead != null)
        {
            //swap directly if a head is detected
            SwapHeads(targetHead);
        }
        else if (targetBody != null)
        {
            //use a threshold to determine if a head is already attached to specific body
            float snapThreshold = 0.3f; //adjust as needed
            PlushHeadSwap attachedHead = null;

            //search through all heads to find one that is currently snapped to this body
            foreach (PlushHeadSwap head in FindObjectsOfType<PlushHeadSwap>())
            {
                if (head != this && Vector3.Distance(head.startPosition, targetBody.transform.position) < snapThreshold)
                {
                    attachedHead = head;
                    break;
                }
            }

            if (attachedHead != null)
            {
                //if there is already a head on this body, swap with that head
                SwapHeads(attachedHead);
            }
            else
            {
                //snap this head to the body's position and update the stored valid position
                transform.position = targetBody.transform.position;
                startPosition = targetBody.transform.position;
            }
        }
        else
        {
            UnityEngine.Debug.Log("Dropped outside valid area, resetting.");
            //if no valid colliders are detected, reset to the last valid position
            transform.position = startPosition;
        }

        //save the new position to SaveManager
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
        }
    }

    private void SwapHeads(PlushHeadSwap otherHead)
    {
        if (otherHead != null && otherHead != this)
        {
            UnityEngine.Debug.Log("Swapping " + gameObject.name + " with " + otherHead.gameObject.name); //was used to look for causes of heads not swapping correctly

            //swap the stored valid positions
            Vector3 tempPosition = startPosition;
            startPosition = otherHead.startPosition;
            otherHead.startPosition = tempPosition;

            //snap each head to its new valid position
            transform.position = startPosition;
            otherHead.transform.position = otherHead.startPosition;

            //save updated positions
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SavePlushiePosition(gameObject.name, transform.position);
                SaveManager.Instance.SavePlushiePosition(otherHead.gameObject.name, otherHead.transform.position);
            }
        }

        //Check if the overall game task is completed after swapping.
        PlushGameManager.Instance.CheckTaskCompletion();
    }

    //check if this head is correctly placed on its matching body
    public bool IsCorrectlyPlaced()
    {
        float snapThreshold = 0.2f;
        return Vector3.Distance(transform.position, correctBody.position) < snapThreshold;
    }

}