using UnityEngine;

public class RotateBehavior : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}

