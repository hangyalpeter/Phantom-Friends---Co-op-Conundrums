using Unity.Netcode;
using UnityEngine;

public class ProjectileSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    public void GetProjectile(Vector3 spawnPoint, Vector3 direction, float speed, float damage, string collisionTag)
    {
        // only server can spawn projectiles, client can't request to spawn projectiles because of latency issues
        if (!IsServer) return;
        SpawnProjectileClientRpc(spawnPoint, direction, speed, damage, collisionTag);

    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc(Vector3 spawnPoint, Vector3 direction, float speed, float damage, string collisionTag)
    {
        if (!IsServer)
        {
            return;
        }

        if (projectilePrefab == null)
        {
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);

        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        networkObject.Spawn(destroyWithScene: true);

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
    }

    [ClientRpc]
    private void SpawnProjectileClientRpc(Vector3 spawnPoint, Vector3 direction, float speed, float damage, string collisionTag)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not set!");
            return;
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
    }

}
