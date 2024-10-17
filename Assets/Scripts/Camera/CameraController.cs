using Unity.Netcode;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float smoothSizeChangeFactor = 0.005f;

    [SerializeField] private Transform childTransform;
    [SerializeField] private Transform ghostTransform;

    [SerializeField] private float smoothTime = 0.1f; 

    private Camera camera;
    private Vector3 cameraVelocity = Vector3.zero;


    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float minCameraSize = 5f;


    private void Start()
    {
        camera = GetComponent<Camera>();
        StartCoroutine(FindPlayers());

    }

    private void FixedUpdate()
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
        Vector3 targetPosition = GetCenterPointOfChildAndGhost();
        targetPosition.z = transform.position.z;

        transform.position = new Vector3(GetCenterPointOfChildAndGhost().x, GetCenterPointOfChildAndGhost().y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref cameraVelocity, smoothTime);

    }

    private Vector2 GetCenterPointOfChildAndGhost()
    {
       return ((childTransform.position + ghostTransform.position) / 2);

    }

     private System.Collections.IEnumerator FindPlayers()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer);

        while (childTransform == null || ghostTransform == null)
        {
            GameObject ghost = GameObject.FindWithTag("Player_Ghost");
            if (ghost != null)
            {
                ghostTransform = ghost.transform;
            }

            GameObject child = GameObject.FindWithTag("Player_Child");
            if (child != null)
            {
                childTransform = child.transform;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
