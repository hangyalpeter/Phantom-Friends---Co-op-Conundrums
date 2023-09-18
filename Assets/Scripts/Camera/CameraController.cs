using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float smoothSizeChangeFactor = 0.005f;

    [SerializeField] private Transform childTransform;
    [SerializeField] private Transform ghostTransform;

    private Camera camera;


    [SerializeField] private float distanceOffset1 = 25;
    [SerializeField] private float distanceOffset2 = 35;
    [SerializeField] private float distanceOffset3 = 45;


    [SerializeField] private float cameraSizeOffset1 = 3;
    [SerializeField] private float cameraSizeOffset2 = 5;
    [SerializeField] private float cameraSizeOffset3 = 7;


    private float originalOrtographicCameraSize;

    private void Start()
    {
        camera = GetComponent<Camera>();
        originalOrtographicCameraSize = camera.orthographicSize;
    }

    private void Update()
    {
        CameraFollowChildAndGhost();

        UpdateCameraSizeAccordingToChildAndGhostDistance();

    }

    private void CameraFollowChildAndGhost()
    {
        transform.position = new Vector3(GetCenterPointOfChildAndGhost().x, GetCenterPointOfChildAndGhost().y, transform.position.z);
    }

    private void UpdateCameraSizeAccordingToChildAndGhostDistance()
    {
        float distanceBetweenChildAndDog = Vector2.Distance(childTransform.position, ghostTransform.position);

        if (distanceBetweenChildAndDog >= distanceOffset1 && camera.orthographicSize <= originalOrtographicCameraSize + cameraSizeOffset1)
        {
            camera.orthographicSize += smoothSizeChangeFactor;
        }
        else if (distanceBetweenChildAndDog >= distanceOffset2 && camera.orthographicSize <= originalOrtographicCameraSize + cameraSizeOffset2)
        {
            camera.orthographicSize += smoothSizeChangeFactor;
        }
        else if (distanceBetweenChildAndDog >= distanceOffset3 && camera.orthographicSize <= originalOrtographicCameraSize + cameraSizeOffset3)
        {
            camera.orthographicSize += smoothSizeChangeFactor;
        }

        else if (distanceBetweenChildAndDog < distanceOffset3 && camera.orthographicSize > originalOrtographicCameraSize)
        {
            camera.orthographicSize -= smoothSizeChangeFactor;
        }
        else if (distanceBetweenChildAndDog < distanceOffset2 && camera.orthographicSize > originalOrtographicCameraSize)
        {
            camera.orthographicSize -= smoothSizeChangeFactor;
        }
        else if (distanceBetweenChildAndDog < distanceOffset1 && camera.orthographicSize > originalOrtographicCameraSize)
        {
            camera.orthographicSize -= smoothSizeChangeFactor;
        }
    }

    private Vector2 GetCenterPointOfChildAndGhost()
    {
        //return new Vector2((childTransform.position.x + ghostTransform.position.x) / 2, (childTransform.position.y + ghostTransform.position.y) / 2);
       return ((childTransform.position + ghostTransform.position) / 2f);

    }
}
