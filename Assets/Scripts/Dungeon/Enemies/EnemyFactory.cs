using UnityEngine;

public class EnemyFactory
{
    public GameObject CreateEnemy(GameObject enemy, Vector3 spawnPosition)
    {

        if (enemy != null)
        {
            GameObject enemyInstance = GameObject.Instantiate(enemy, spawnPosition, Quaternion.identity);
            return enemyInstance;
        }

        return null;
    }
}

