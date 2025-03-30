using UnityEngine;

public class FishFoodDrag : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 originalPos;
    private bool isDragging = false;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
            transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        isDragging = false;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Fish"))
            {
                // Notify manager
                FindObjectOfType<FishManager>().FeedFish();
                return;
            }
        }

        // Return to start if not dropped on fish
        transform.position = originalPos;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        return mouse;
    }
}

