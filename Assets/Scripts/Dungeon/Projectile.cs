using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed = 10f;
    public float damage = 25f;

    private Rigidbody2D rb;


     void Start()
    {
         rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Set the velocity of the projectile in the direction of movement which is the player atm
            Vector3 direction = (GameObject.Find("Player_Child").transform.position - transform.position).normalized;

            rb.velocity = direction * speed;
        }
        else
        {
            Debug.LogError("No Rigidbody2D found on projectile.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        if (health != null && !collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        } else
        {
            Destroy(gameObject);
        }

    }


}
