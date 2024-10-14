using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IHealthProvider
{
    public float maxHealth = 100f;
    private float currentHealth;

    public event Action OnDied;
    public event Action<float> OnHealthChanged;
    public static event Action OnEnemyDied;

    private bool dieInvoked = false;

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
        if (gameObject.CompareTag("Player_Child") || gameObject.CompareTag("PossessedEnemy") || gameObject.CompareTag("Possessable"))
        {
            OnDied?.Invoke();
            if (gameObject.GetComponent<PosessableMovement>().IsPossessed)
            {
                FindAnyObjectByType<PossessionTimer>()?.StopTimer();
            }
            Destroy(gameObject);
            return;
        }
        else
        {
            OnEnemyDied?.Invoke();
            Destroy(gameObject);
        }
    }
}


