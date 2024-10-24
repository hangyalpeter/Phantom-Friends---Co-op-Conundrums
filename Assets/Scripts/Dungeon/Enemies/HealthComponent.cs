using System;
using Unity.Netcode;
using UnityEngine;

public class HealthComponent : NetworkBehaviour, IHealthProvider
{
    public float maxHealth = 100f;
    private float currentHealth;

    public event Action OnDied;
    public event Action<float> OnHealthChanged;
    public static event Action OnEnemyDied;
    public static Action OnPossessedObjectDies;

    private bool dieInvoked = false;

    private bool canDespawn = true;

    public float MaxHealth => maxHealth;

    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                OnHealthChanged?.Invoke(currentHealth);
            }
        }
    }

    void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0 && !dieInvoked)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }

    private void Die()
    {
        dieInvoked = true;
        ulong networkObjectId = gameObject.GetComponent<NetworkObject>().NetworkObjectId;

        // Make sure player is not possessable!
        if (gameObject.CompareTag("Player_Child") || gameObject.GetComponent<PossessableTransformation>() != null)
        {
            OnDied?.Invoke();
            if (gameObject.CompareTag("Enemy"))
            {
                OnEnemyDied?.Invoke();
            }
            if (gameObject.GetComponent<PosessableMovement>().IsPossessed)
            {
                OnPossessedObjectDies?.Invoke();
            }
            DespawnDeadObject(networkObjectId);
        }
        else
        {
            OnEnemyDied?.Invoke();

            DespawnDeadObject(networkObjectId);
        }
    }

    private void DespawnDeadObject(ulong networkObjectId)
    {
        if (!IsServer && IsClient)
        {
            RequestDestroyOnServerRpc(networkObjectId);
        }
        else
        {
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (networkObject == null) Debug.LogError("Networkobject null");
            if (networkObject != null)
            {
                networkObject.Despawn(networkObject);
            }

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyOnServerRpc(ulong networkObjectId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (networkObject != null)
        {
            networkObject.Despawn(networkObject);
        }
    }
}


