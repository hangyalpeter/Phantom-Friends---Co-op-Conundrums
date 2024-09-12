using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed = 10f;
    public float damage = 25f;

    private Vector3 direction;

    void Start()
    {

        // TODO: find another way for finding the player, maybe with the findobjectwithtag is better
        direction = (GameObject.Find("Player_Child").transform.position - transform.position).normalized;
    }
    private void Update()
    {

        var step = speed * Time.deltaTime;
        transform.position = transform.position + direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
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
