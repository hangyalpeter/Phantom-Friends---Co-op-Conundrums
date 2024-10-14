using UnityEngine;

public class MovementRestriction : MonoBehaviour
{
    public BoxCollider2D boundary; // Reference to the 2D boundary collider
    private Vector2 minBoundary;
    private Vector2 maxBoundary;

    private void Start()
    {
        // Get the boundaries from the BoxCollider2D
        Vector2 boundaryCenter = boundary.bounds.center;
        Vector2 boundarySize = boundary.bounds.extents; // Half the size gives us the extents
        minBoundary = boundaryCenter - boundarySize;
        maxBoundary = boundaryCenter + boundarySize;
    }

    private void Update()
    {
        // Restrict movement within the boundaries in 2D space
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBoundary.x, maxBoundary.x),
            Mathf.Clamp(transform.position.y, minBoundary.y, maxBoundary.y),
            transform.position.z // No clamping on the Z axis since it's 2D
        );
    }

    // Optional: Draw the boundaries in the editor view for visualization
    private void OnDrawGizmos()
    {
        if (boundary != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundary.bounds.center, boundary.bounds.size);
        }
    }}

