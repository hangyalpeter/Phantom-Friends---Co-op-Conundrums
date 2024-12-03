using System;
using Unity.Netcode;
using UnityEngine;

public abstract class HealthBase : NetworkBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float MaxHealth => maxHealth;

    public Action OnDied;
    public event Action<float> OnHealthChanged;

    protected bool dieInvoked = false;
    protected NetworkVariable<float> currentHealthNetwork = new NetworkVariable<float>();

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHealthNetwork.OnValueChanged += (prev, curr) =>
        {
            CurrentHealth = curr;
        };
    }

    protected virtual void Start()
    {
        if (IsServer)
        {
            currentHealthNetwork.Value = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            currentHealthNetwork.Value -= damage;
        }

        if (CurrentHealth <= 0 && !dieInvoked)
        {
            Die();
        }
    }

    protected void DespawnDeadObject(ulong networkObjectId)
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

    public void ResetHealth()
    {
        currentHealthNetwork.Value = maxHealth;
    }

    protected abstract void Die();
}

