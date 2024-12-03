using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    private IDungeonMediator mediator;

    private Transform player;
    private Transform ghost;

    private PlayerHealth healthProvider;

    public Transform Player => player;
    public Transform Ghost => ghost;

    private GameObject playerGO;

    private PlayerMovementDungeon playerMovementDungeon;
    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(FindObjectsAfterSpawn());
    }

    private IEnumerator FindObjectsAfterSpawn()
    {
        while (GameObject.FindGameObjectWithTag("Player_Child") == null || GameObject.FindGameObjectWithTag("Player_Ghost") == null)
        {
            yield return null;
        }

        playerGO = GameObject.FindGameObjectWithTag("Player_Child");
        playerMovementDungeon = playerGO.GetComponent<PlayerMovementDungeon>();
        GameObject ghostGO = GameObject.FindGameObjectWithTag("Player_Ghost");
        player = playerGO.transform;
        ghost = ghostGO.transform;
        healthProvider = playerGO.GetComponent<PlayerHealth>();
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
        playerGO.GetComponent<PlayerHealth>().ResetHealth();
    }

    public void IncreaseDamage(float damage)
    {
        playerMovementDungeon.Damage = playerMovementDungeon.Damage * damage;

    }

}
