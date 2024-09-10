using System;
using UnityEngine;
public class ShootBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float interval = 2f;  // Time between shots
    private float nextShotTime = 0f;


    public event Action OnShoot;

    void Update()
    {
        if (Time.time >= nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + interval;
        }
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
}

