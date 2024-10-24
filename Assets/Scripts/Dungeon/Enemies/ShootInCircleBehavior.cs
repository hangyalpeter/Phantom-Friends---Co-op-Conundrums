using UnityEngine;

public class ShootInCircleBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;

    private float spawnRadius = 1.0f;
    private float projectileSpeed = 10f;

    private int numberOfProjectiles = 10;

    // TODO maybe make it a parameter for enemydata
    private float spawnInterval = 3f;

    private string collisionTag = "Enemy";

    private ProjectileSpawner projectileSpawner;

    public float damage = 25f;

    private void Start()
    {
        projectileSpawner = GetComponent<ProjectileSpawner>();
        InvokeRepeating(nameof(FireAllProjectilesInCircle), 0f, spawnInterval);
    }

    private void FireAllProjectilesInCircle()
    {
        float angleStep = 360f / numberOfProjectiles;
        float currentAngle = 0f;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angleInRadians = currentAngle * Mathf.Deg2Rad;
            Vector2 spawnDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;

            Vector2 spawnPoint = (Vector2)transform.position + spawnDirection * spawnRadius;


            if (ProjectileFactory.Instance != null)
            {
                projectileSpawner.GetProjectile(spawnPoint, spawnDirection, projectileSpeed, damage, collisionTag);
            }

            currentAngle += angleStep;
        }
    }
}
