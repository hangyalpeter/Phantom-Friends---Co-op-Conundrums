using UnityEngine;

public class PlayerHealth : HealthBase
{
    protected override void Die()
    {
        dieInvoked = true;
        OnDied?.Invoke();
        Debug.Log("Player has died.");
        DespawnDeadObject(NetworkObjectId);
    }
}

