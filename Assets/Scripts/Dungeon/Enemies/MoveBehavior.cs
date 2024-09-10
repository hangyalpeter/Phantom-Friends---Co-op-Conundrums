using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    public float speed;

    public Transform target;


    void Update()
    {

        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        AlwaysFacePlayer();
    }

    private void AlwaysFacePlayer()
    {
        if (transform.position.x > target.position.x)
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        }
    }

  }

