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

    public GameObject CreateEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.name = enemyName;

        // the builder checks if it can add the corresponding components
        EnemyBuilder builder = new EnemyBuilder(enemy, this);
        builder.WithHealth()
               .WithMovement()
               .WithShooting()
               .withPossessable()
               .Build();

        return enemy;
    }
}