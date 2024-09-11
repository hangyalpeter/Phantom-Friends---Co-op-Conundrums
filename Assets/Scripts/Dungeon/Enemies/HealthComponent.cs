﻿using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public event Action OnDied;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}


