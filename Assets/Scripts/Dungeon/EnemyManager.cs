﻿using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private IDungeonMediator mediator;

    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDied += Die;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= Die;
    }

    private void Die()
    {
        mediator.Notify(this, DungeonEvents.EnemyDied);
    }

}