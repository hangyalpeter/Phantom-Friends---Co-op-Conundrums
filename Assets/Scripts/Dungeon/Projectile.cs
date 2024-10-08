using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float damage = 25f;

    [SerializeField]
    private string collisionTag;

    public void SetCollisionTag(string tag)
    {
        collisionTag = tag;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealthProvider health = collision.gameObject.GetComponent<IHealthProvider>();

        if (collision.gameObject.CompareTag(collisionTag))
        {
            return;

        }
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
