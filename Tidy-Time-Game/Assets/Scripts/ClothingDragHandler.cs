using UnityEngine;

public class ClothingDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isPlaced = false;

    public string targetZoneTag;
    public GameObject organizedVersion; 

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (isPlaced) return;
        offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        if (isPlaced) return;
        transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        if (isPlaced) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(targetZoneTag))
            {
                //snap into place and show organized version
                isPlaced = true;
                if (organizedVersion != null)
                {
                    organizedVersion.SetActive(true);
                }
                gameObject.SetActive(false); //hide current piece
                return;
            }
        }

        //place back if dropped in wrong spot
        transform.position = originalPosition;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }
}