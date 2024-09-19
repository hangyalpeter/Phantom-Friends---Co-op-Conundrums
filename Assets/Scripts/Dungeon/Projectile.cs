using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed = 10f;
    public float damage = 25f;

    public float colliderEnableDelay = 1f;  // Delay before enabling the collider

    private Collider2D bulletCollider;

    void Start()
    {
        bulletCollider = GetComponent<Collider2D>();

        bulletCollider.enabled = false;

        // Enable it after a short delay
        Invoke(nameof(EnableCollider), colliderEnableDelay);
    }

    void EnableCollider()
    {
        bulletCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("collision with enemy");
            return;

        }
        if (health != null && !collision.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
        }

        Debug.Log("no collision with enemy");

        Destroy(gameObject);
    }

}
