using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float smoothSizeChangeFactor = 0.005f;

    [SerializeField] private Transform childTransform;
    [SerializeField] private Transform ghostTransform;

    private Camera camera;

    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minCameraSize = 5f;


    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (childTransform != null && ghostTransform != null)
        {
            CameraFollowChildAndGhost();
            UpdateCameraSize();
        }

    }

    private void UpdateCameraSize()
    {
        float distance = Vector3.Distance(childTransform.position, ghostTransform.position);

        float targetCameraSize = distance / 2.0f;

        targetCameraSize = Mathf.Max(targetCameraSize, minCameraSize);

        float newCameraSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, Time.deltaTime * zoomSpeed);
        camera.orthographicSize = newCameraSize;
    }

    private void CameraFollowChildAndGhost()
    {
        transform.position = new Vector3(GetCenterPointOfChildAndGhost().x, GetCenterPointOfChildAndGhost().y, transform.position.z);
    }

    private Vector2 GetCenterPointOfChildAndGhost()
    {
       return ((childTransform.position + ghostTransform.position) / 2f);

    }
}
