using System;
using Unity.Netcode;
using UnityEngine;

public class PossessableHealth : HealthBase
{
    public static Action OnPossessedObjectDies;

    protected override void Die()
    {
        dieInvoked = true;
        if (GetComponent<PosessableMovement>().IsPossessed)
        {
            OnPossessedObjectDies?.Invoke();
        }
        Debug.Log("Possessable object has died.");
        DespawnDeadObject(NetworkObjectId);
    }

}

