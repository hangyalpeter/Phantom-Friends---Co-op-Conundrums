using System;
using UnityEngine;
public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;

    // TODO: fix access levels
    public float interval = 10f;
    private float nextShotTime = 0f;

    public float speed = 10f;
    public float damage = 25f;

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Vector3 lastMovementDirection;
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
            var direction = GetComponent<FollowPlayerBehavior>() == null ? (target.position - transform.position) : lastMovementDirection;
            ProjectileFactory.Instance.GetProjectile(projectilePrefab, shootingPoint.position, direction, speed, "Enemy");

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

        if (direction != Vector3.zero)
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

