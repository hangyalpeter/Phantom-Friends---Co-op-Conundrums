using System;
using UnityEngine;
public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;

    // TODO: fix access levels
    public float interval = 10f;  // Time between shots
    private float nextShotTime = 0f;

    public float speed = 10f;
    public float damage = 25f;

    private Vector2 currentPosition;
    private Vector2 previousPosition;
    private Vector2 lastMovementDirection;
    public Transform target { get; internal set; }

    public event Action OnShoot;

    void Update()
    {
        UpdateShootingDirection();
        if (Time.time >= nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + interval;
        }
        AlwaysFacePlayer();
    }

    private void Shoot()
    {
        if (projectilePrefab != null && shootingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);
            Projectile projComponent = projectile.GetComponent<Projectile>();
            if (projComponent != null)
            {
                //projComponent.speed = speed;
                projComponent.damage = damage;
                if (GetComponent<FollowPlayerBehavior>() != null)
                {
                    projComponent.GetComponent<Rigidbody2D>().velocity = lastMovementDirection * speed;
                } else
                {
                    projComponent.GetComponent<Rigidbody2D>().velocity = (target.position - transform.position).normalized * speed;

                }
            }

            OnShoot?.Invoke();
        }
        else
        {
            Debug.LogWarning("Projectile prefab or shooting point not set!");
        }

    }
    

    private void UpdateShootingDirection()
    {
        currentPosition = transform.position;
        var direction = currentPosition - previousPosition;

        if (direction != Vector2.zero)
        {
            lastMovementDirection = direction.normalized;
        }


        previousPosition = currentPosition;
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

