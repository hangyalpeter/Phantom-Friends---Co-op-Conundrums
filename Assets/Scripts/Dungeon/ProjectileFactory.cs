using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public static ProjectileFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject GetProjectile(GameObject projectilePrefab, Vector3 spawnPoint, Vector3 direction, float speed, float damage, string collisionTag)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not set!");
            return null;
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.damage = damage;
            projectileComponent.SetCollisionTag(collisionTag);
        }

        return projectile;
    }
}
