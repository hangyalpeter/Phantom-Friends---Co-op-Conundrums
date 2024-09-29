using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public event Action OnDied;
    public event Action OnDamageTaken;
    public event Action<float> OnDamageTakenWithAmount;

    public float MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + currentHealth);

        OnDamageTaken?.Invoke();
        OnDamageTakenWithAmount?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        OnDied?.Invoke();

        if (gameObject.tag == "Player_Child")
        {
            return;
        }
        else
        {
            Destroy(gameObject, 0.2f);
        }
    }
}


