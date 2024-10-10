using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private IDungeonMediator mediator;

    private Transform player;
    private Transform ghost;

    private IHealthProvider healthProvider;

    public Transform Player => player;
    public Transform Ghost => ghost;
    
    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void OnEnable()
    {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player_Child");
        GameObject ghostGO = GameObject.FindGameObjectWithTag("Player_Ghost");
        player = playerGO.transform;
        ghost = ghostGO.transform;
        healthProvider = playerGO.GetComponent<IHealthProvider>();
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
