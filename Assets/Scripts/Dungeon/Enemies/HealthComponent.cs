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

    public float CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && !dieInvoked)
        {
            Die();
        }
    }

    private void Die()
    {

        dieInvoked = true;
        if (gameObject.tag == "Player_Child" || gameObject.tag == "PossessedEnemy" || gameObject.tag == "Possessable")
        {
            OnDied?.Invoke();
            FindAnyObjectByType<PossessionTimer>()?.StopTimer();
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


