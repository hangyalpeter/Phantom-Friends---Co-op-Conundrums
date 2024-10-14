using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private IDungeonMediator mediator;

    private Transform player;
    private Transform ghost;

    private IHealthProvider healthProvider;

    public Transform Player => player;
    public Transform Ghost => ghost;

    private GameObject playerGO;

    private PlayerMovement1 playerMovement1;
    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void OnEnable()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player_Child");
        playerMovement1 = playerGO.GetComponent<PlayerMovement1>();
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

    public void ResetHealth()
    {
        playerGO.GetComponent<HealthComponent>().ResetHealth();
    }

    public void IncreaseDamage(float damage)
    {
        playerMovement1.Damage = playerMovement1.Damage * damage;

    }

}
