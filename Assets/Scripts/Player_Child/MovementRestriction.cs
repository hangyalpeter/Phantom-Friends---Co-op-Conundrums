using UnityEngine;

public class MovementRestriction : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D boundary; // Reference to the 2D boundary collider
    private Vector2 minBoundary;
    private Vector2 maxBoundary;

    private void Start()
    {
        Vector2 boundaryCenter = boundary.bounds.center;
        Vector2 boundarySize = boundary.bounds.extents;
        minBoundary = boundaryCenter - boundarySize;
        maxBoundary = boundaryCenter + boundarySize;
    }

    private void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBoundary.x, maxBoundary.x),
            Mathf.Clamp(transform.position.y, minBoundary.y, maxBoundary.y),
            transform.position.z 
        );
    }

    private void OnDrawGizmos()
    {
        if (boundary != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundary.bounds.center, boundary.bounds.size);
        }
    }}

