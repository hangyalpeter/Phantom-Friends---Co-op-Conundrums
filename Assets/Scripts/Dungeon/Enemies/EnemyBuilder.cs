using UnityEngine;

public class EnemyBuilder
{
    private GameObject enemyInstance;

    public EnemyBuilder(GameObject enemyInstance)
    {
        this.enemyInstance = enemyInstance;
    }

    public EnemyBuilder AddMoveBehavior(float speed)
    {
        MoveBehavior moveBehavior = enemyInstance.AddComponent<MoveBehavior>();
        moveBehavior.speed = speed;
        // TODO: add player's position as target as function parameter instead in enemy spawner and in enemy spawner, player should be a [serializefield]
        moveBehavior.target = GameObject.Find("Player_Child").transform;
        return this;
    }

    public EnemyBuilder AddShootBehavior(float shootInterval)
    {
        ShootBehavior shootBehavior = enemyInstance.AddComponent<ShootBehavior>();
        shootBehavior.interval = shootInterval;
        return this;
    }

    public EnemyBuilder AddRotateBehavior(float rotateSpeed)
    {
        RotateBehavior rotateBehavior = enemyInstance.AddComponent<RotateBehavior>();
        rotateBehavior.speed = rotateSpeed;
        return this;
    }

    public EnemyBuilder AddHealth(float maxHealth)
    {
        HealthComponent healthComponent = enemyInstance.AddComponent<HealthComponent>();
        healthComponent.maxHealth = maxHealth;
        return this;
    }

    public GameObject Build()
    {
        return enemyInstance;
    }


}

