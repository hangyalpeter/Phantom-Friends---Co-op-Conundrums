using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;   // Reference to the prefab
    public float health;
    public float speed;
    public float damage;
    public GameObject projectilePrefab;  // If the enemy can shoot
    public float shootInterval;
    public float projectileSpeed;
    public bool canMove;
    public bool canShoot;
    public bool canBePossessed;
    public bool canRotateShoot;
    public bool canShootInCircle;
    public bool isBoss;
    public GameObject CreateEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

        NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
        enemy.name = enemyName;
        // the builder checks if it can add the corresponding components
        EnemyBuilder builder = new EnemyBuilder(enemy, this);

        if (!isBoss)
        {
            builder.WithHealth()
                   .WithMovement()
                   .WithShooting()
                   .WithRotateShooting()
                   .withPossessable()
                   .WithShootInCircle()
                   .Build();
        }
        else
        {
            builder.WithHealth()
                   .Build();
        }

        networkObject.Spawn(destroyWithScene: true);
        return enemy;
    }
}