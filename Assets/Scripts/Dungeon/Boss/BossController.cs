using UnityEngine;

public class BossController : MonoBehaviour
{
    private IBossState currentState;

    private EnemyBuilder enemyBuilder;

    [SerializeField]
    private EnemyData enemyData;

    private void Awake()
    {
        // Create the EnemyBuilder based on the current boss instance
        enemyBuilder = new EnemyBuilder(this.gameObject, enemyData);
    }

    public void TransitionToState(IBossState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }

        currentState = newState;
        currentState.EnterState(this, enemyBuilder);
    }

    public void Update()
    {
        currentState?.UpdateState(this);
    }

    public void AddBehavior<T>() where T : Component
    {
        if (GetComponent<T>() == null)
        {
            gameObject.AddComponent<T>();
        }
    }

    public void RemoveBehavior<T>() where T : Component
    {
        T behavior = GetComponent<T>();
        if (behavior != null)
        {
            Destroy(behavior);
        }
    }
}

