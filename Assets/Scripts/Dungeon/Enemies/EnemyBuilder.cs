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
        if (enemyData.canMove)
        {
            FollowPlayerBehavior followPlayerBehaviour = enemyInstance.AddComponent<FollowPlayerBehavior>();
            followPlayerBehaviour.speed = enemyData.speed;
            // TODO: add player's position as target as function parameter or some other solution than this instead in enemy spawner and in enemy spawner, player should be a [serializefield]
            followPlayerBehaviour.target = GameObject.FindGameObjectWithTag("Player_Child").transform;
        }
        return this;
    }

    public EnemyBuilder WithShooting()
    {
        if (enemyData.canShoot)
        {
            ShootBehavior shootBehavior = enemyInstance.AddComponent<ShootBehavior>();
            shootBehavior.interval = enemyData.shootInterval;
            shootBehavior.target = GameObject.FindGameObjectWithTag("Player_Child").transform;
            shootBehavior.projectilePrefab = enemyData.projectilePrefab;
            shootBehavior.shootingPoint = enemyInstance.transform;
            shootBehavior.damage = enemyData.damage;
            shootBehavior.speed = enemyData.projectileSpeed;
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

    public EnemyBuilder AddRotateBehavior(float rotateSpeed)
    {
        RotateBehavior rotateBehavior = enemyInstance.AddComponent<RotateBehavior>();
        return this;
    }

    // TODO observable for healthbar
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


}

