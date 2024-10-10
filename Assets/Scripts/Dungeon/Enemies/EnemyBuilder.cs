using UnityEngine;

public class EnemyBuilder
{
    private GameObject enemyInstance;
    private EnemyData enemyData;

    public EnemyBuilder(GameObject enemyInstance, EnemyData enemyData)
    {
        this.enemyInstance = enemyInstance;
        this.enemyData = enemyData;
    }

    public EnemyBuilder WithMovement()
    {
        if (enemyData.canMove || enemyData.isBoss)
        {
            FollowPlayerBehavior followPlayerBehaviour = enemyInstance.AddComponent<FollowPlayerBehavior>();
            followPlayerBehaviour.speed = enemyData.speed;
            followPlayerBehaviour.target = GameObject.FindGameObjectWithTag("Player_Child").transform;
        }
        return this;
    }

    public EnemyBuilder WithShooting()
    {
        if (enemyData.canShoot || enemyData.isBoss)
        {
            ShootBehavior shootBehavior = enemyInstance.AddComponent<ShootBehavior>();
            shootBehavior.target = GameObject.FindGameObjectWithTag("Player_Child").transform;
            shootBehavior.projectilePrefab = enemyData.projectilePrefab;
            shootBehavior.shootingPoint = enemyInstance.transform;
            shootBehavior.damage = enemyData.damage;
            shootBehavior.speed = enemyData.projectileSpeed;
            shootBehavior.spawnInterval = enemyData.shootInterval;
        }
       return this;
    }

    public EnemyBuilder withPossessable()
    {
        if (enemyData.canBePossessed)
        {
            enemyInstance.AddComponent<PossessableBehavior>();
            enemyInstance.AddComponent<PossessableTransformation>();
            enemyInstance.AddComponent<PosessableMovement>();
        }
        return this;
    }

    public EnemyBuilder WithHealth()
    {
        HealthComponent healthComponent = enemyInstance.AddComponent<HealthComponent>();
        healthComponent.maxHealth = enemyData.health;
        return this;
    }

    public GameObject Build()
    {
        return enemyInstance;
    }

    public EnemyBuilder WithRotateShooting()
    {
        if (enemyInstance.GetComponent<RotateShootingBehavior>() == null)
        {
            if (enemyData.canRotateShoot || enemyData.isBoss)
            {
                RotateShootingBehavior rotateBehavior = enemyInstance.AddComponent<RotateShootingBehavior>();
                rotateBehavior.projectilePrefab = enemyData.projectilePrefab;
                rotateBehavior.damage = enemyData.damage;
            }
        }
        return this;
    }

    public EnemyBuilder WithShootInCircle()
    {
        if (enemyInstance.GetComponent<ShootInCircleBehavior>() == null)
        {
            if (enemyData.canShootInCircle || enemyData.isBoss)
            {
                ShootInCircleBehavior shootInCircleBehavior = enemyInstance.AddComponent<ShootInCircleBehavior>();
                shootInCircleBehavior.projectilePrefab = enemyData.projectilePrefab;
                shootInCircleBehavior.damage = enemyData.damage;
            }
        }
        return this;
    }
}

