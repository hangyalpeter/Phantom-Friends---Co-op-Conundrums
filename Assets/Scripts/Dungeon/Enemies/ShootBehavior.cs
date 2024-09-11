using System;
using UnityEngine;
public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;

    // TODO: fix access levels
    public float interval = 10f;  // Time between shots
    private float nextShotTime = 0f;

    public Transform target { get; internal set; }

    public event Action OnShoot;

    void Update()
    {
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
                projComponent.speed = 10f;
                projComponent.damage = 25f;
            }

            OnShoot?.Invoke();
        }
        else
        {
            Debug.LogWarning("Projectile prefab or shooting point not set!");
        }

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

