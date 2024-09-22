using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    private HealthComponent healthComponent;
    void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent != null )
        {
            healthComponent.OnDamageTakenWithAmount += SetHealth;
            SetMaxHealth(healthComponent.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDamageTakenWithAmount -= SetHealth;
        }

    }

    private void SetMaxHealth(float health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

    }

    private void SetHealth(float health)
    {
        healthSlider.value = health;
    }


   
}
