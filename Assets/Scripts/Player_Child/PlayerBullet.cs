using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: remove class
public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    private float damage = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealthProvider health = collision.gameObject.GetComponent<IHealthProvider>();
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            return;
        }

        if (health != null && collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        } else
        {
            Destroy(gameObject);
        }


    }
}
