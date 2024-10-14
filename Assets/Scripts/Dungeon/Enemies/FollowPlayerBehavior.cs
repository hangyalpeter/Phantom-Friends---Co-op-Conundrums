using System.Collections;
using UnityEngine;

public class FollowPlayerBehavior : MonoBehaviour
{
    public float speed;

    public Transform target;
    public Transform secondTarget;
    private Transform currentTarget;

    private void Start()
    {
        currentTarget = target;
        StartCoroutine(ChangeTarget());
    }

    private void OnDisable()
    {
        StopCoroutine(ChangeTarget());
    }

    void Update()
    {

        var step = speed * Time.deltaTime;
        if (target == null || secondTarget == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, currentTarget.position) > 4f)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, step);
        }
        AlwaysFacePlayer();
    }

    private IEnumerator ChangeTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            float randomValue = Random.Range(0f, 1f); // Generate a random float between 0 and 1

            if (randomValue <= 0.6f)
            {
                currentTarget = target;
            }
            else
            {
                currentTarget = secondTarget;
            }
        }

    }

    private void AlwaysFacePlayer()
    {
        if (transform.position.x > currentTarget.position.x)
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }
        else
        {
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        }
    }

  }

