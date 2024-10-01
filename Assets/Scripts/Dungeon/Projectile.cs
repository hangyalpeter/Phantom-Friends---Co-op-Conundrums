using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed = 10f;
    public float damage = 25f;

    [SerializeField]
    private string collisionTag;

    public void SetCollisionTag(string tag)
    {
        collisionTag = tag;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();

        if (collision.gameObject.CompareTag(collisionTag))
        {
            Debug.Log("collision with enemy");
            return;

        }
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Debug.Log("no collision with enemy");

        Destroy(gameObject);
    }

}
