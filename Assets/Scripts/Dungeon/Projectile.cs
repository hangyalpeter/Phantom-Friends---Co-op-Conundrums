using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
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

        if (collision.gameObject.CompareTag(collisionTag) || collision.gameObject.CompareTag("Player_Ghost"))
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
