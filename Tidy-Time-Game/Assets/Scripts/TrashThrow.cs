using System.Collections;
using UnityEngine;

public class TrashThrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool isDragging = false;
    private LineRenderer lineRenderer;

    public float forceMultiplier = 6f;  //launch force 
    public int lineSegmentCount = 15;  //number of points in trajectory arc
    public float timeStep = 0.1f;  //time between each arc point

    private Vector3 originalPosition;
    private bool hasLaunched = false;

    void Start()
    {
        //save start position for resetting missed throws
        originalPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        //set Rigidbody to kinematic at start to prevent falling
        rb.bodyType = RigidbodyType2D.Kinematic;

        lineRenderer.enabled = false;
    }

    void OnMouseDown()
    {
        startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        lineRenderer.enabled = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = transform.position.z;
            transform.position = currentPos;

            //show predicted throw trajectory
            Vector2 dragVector = startPos - (Vector2)currentPos;
            DrawTrajectory(transform.position, dragVector * forceMultiplier);
        }
    }

    void OnMouseUp()
    {
        //determine launch direction
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchDir = startPos - endPos;

        hasLaunched = true;

        //change Rigidbody to dynamic when throwing to enable physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        //apply force based on drag distance and direction
        rb.AddForce(launchDir * forceMultiplier, ForceMode2D.Impulse);

        //change to active layer to avoid interaction with idle trash pieces
        gameObject.layer = LayerMask.NameToLayer("TrashActive");

        isDragging = false;
        lineRenderer.enabled = false;
    }
 
    //simulate physics arc and draw it
    void DrawTrajectory(Vector2 startPoint, Vector2 velocity)
    {
        Vector3[] points = new Vector3[lineSegmentCount];

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float t = i * timeStep;
            float dx = velocity.x * t;
            float dy = velocity.y * t + 0.5f * Physics2D.gravity.y * t * t;

            points[i] = new Vector3(startPoint.x + dx, startPoint.y + dy, transform.position.z);
        }

        lineRenderer.positionCount = lineSegmentCount;
        lineRenderer.SetPositions(points);
    }

    //reset position of trash if it hits the border/misses the bin
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLaunched && collision.collider.CompareTag("SceneBorder"))
        {
            StartCoroutine(ResetTrash());
        }
    }

    //move trash pieces to original position and switch back to kinematic
    private IEnumerator ResetTrash()
    {
        yield return new WaitForSeconds(0.5f); //delay before resetting

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        transform.position = originalPosition;
        hasLaunched = false;
    }
}
