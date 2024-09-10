using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyFactory enemyFactory;
    private GameObject trunkEnemy;


    void Start()
    {
        enemyFactory = new EnemyFactory();

        trunkEnemy = Resources.Load<GameObject>("Trunk_Enemy");

        if (trunkEnemy == null)
        {
            Debug.LogError("Prefab could not be loaded. Check the file paths and ensure the prefabs are in the Resources folder.");
            return;
        }

    }

    public void SpawnEnemies(Vector3 position)
    {
        GameObject movingEnemy = enemyFactory.CreateEnemy(trunkEnemy, position);

        // here we can maybe assign a random value if it can shoot or not.

        EnemyBuilder movingBuilder = new EnemyBuilder(movingEnemy);
        movingBuilder
            .AddMoveBehavior(3f)
            .AddShootBehavior(1f)
            .AddHealth(100f)
            .Build();

        ShootBehavior shootBehavior = movingEnemy.GetComponent<ShootBehavior>();
        if (shootBehavior != null)
        {
            shootBehavior.projectilePrefab = Resources.Load<GameObject>("Trunk_Enemy_Bullet");
            shootBehavior.shootingPoint = movingEnemy.transform;
        }
    }
}

