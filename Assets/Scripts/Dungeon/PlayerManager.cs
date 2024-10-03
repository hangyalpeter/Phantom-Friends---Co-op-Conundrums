using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private IDungeonMediator mediator;

    private Transform player;

    private IHealthProvider healthProvider;

    public Transform Player => player;
    
    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void OnEnable()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player_Child");
        player = playerGO.transform;
        healthProvider = playerGO.GetComponent<HealthComponent>();
        healthProvider.OnDied += Die;
    }

    private void OnDisable()
    {
        if (healthProvider != null)
        {
            healthProvider.OnDied -= Die;
        }
    }

    private void Die()
    {
        mediator.Notify(this, DungeonEvents.PlayerDied);
    }
}
