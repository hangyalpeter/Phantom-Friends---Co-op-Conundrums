using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    private float damage = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            return;
        }

        if (health != null && collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
            var tea = collision.gameObject.GetComponent<TrunkEnemyAnimationHandler>();
            if (tea != null)
            {
                tea.PlayHitAnimation();
            }

            Destroy(gameObject);
        } else
        {
            Destroy(gameObject);
        }


    }
}
