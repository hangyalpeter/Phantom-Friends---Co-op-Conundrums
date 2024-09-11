using System.Collections.Generic;
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

    public List<GameObject> SpawnEnemies(Vector3 position)
    {
        var spawnedEnemiesInCurrentRoom = new List<GameObject>();
        GameObject movingEnemy = enemyFactory.CreateEnemy(trunkEnemy, position);

        GameObject stationaryEnemy = enemyFactory.CreateEnemy(trunkEnemy, position + new Vector3(0.3f, 0, position.z));

        // here we can maybe assign a random value if it can shoot or not.

        EnemyBuilder movingBuilder = new EnemyBuilder(movingEnemy);
        movingBuilder
            .AddMoveBehavior(3f)
            .AddShootBehavior(10f)
            .AddHealth(100f)
            .Build();

        EnemyBuilder stationaryBuilder = new EnemyBuilder(stationaryEnemy);
        stationaryBuilder
            .AddShootBehavior(3f)
            .AddHealth(200f)
            .Build();

        InitializeShootingBehaviour(movingEnemy);

        InitializeShootingBehaviour(stationaryEnemy);

        spawnedEnemiesInCurrentRoom.Add(movingEnemy);
        spawnedEnemiesInCurrentRoom.Add(stationaryEnemy);

        return spawnedEnemiesInCurrentRoom;

    }

    // TODO: refactor this into the shootbehavior i think
    private static void InitializeShootingBehaviour(GameObject enemy)
    {
        ShootBehavior shootBehavior = enemy.GetComponent<ShootBehavior>();
        if (shootBehavior != null)
        {
            shootBehavior.projectilePrefab = Resources.Load<GameObject>("Trunk_Enemy_Bullet");
            shootBehavior.shootingPoint = enemy.transform;
        }
    }
}

