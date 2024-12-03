using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject SpawnEnemy(EnemyData enemyData, Vector3 position)
    {
        return enemyData.CreateEnemy(position);
    }
}

