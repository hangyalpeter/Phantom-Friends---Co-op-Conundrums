using System;
using UnityEngine;

public class EnemyHealth : HealthBase
{
    public static event Action OnEnemyDied;

    protected override void Die()
    {
        dieInvoked = true;
        if (GetComponent<PossessableTransformation>() != null && GetComponent<PosessableMovement>().IsPossessed)
        {
            PossessableHealth.OnPossessedObjectDies?.Invoke();
        }
        OnEnemyDied?.Invoke();
        Debug.Log("Enemy has died.");
        DespawnDeadObject(NetworkObjectId);
    }
}

