using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FallingTrap : NetworkBehaviour
{
    private Rigidbody2D rb;
    private bool isAlreadyTriggered = false;
    [SerializeField] private float fallingTimeOffset = 0.5f;
    private BoxCollider2D bc;
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       bc = GetComponent<BoxCollider2D>();
    }
    private IEnumerator FallingTrapCoroutine()
    {
        yield return new WaitForSeconds(fallingTimeOffset);
        rb.isKinematic = false;
        isAlreadyTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            if (isAlreadyTriggered)
            {
                return;
            }
            StartCoroutine(FallingTrapCoroutine());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Child") || collision.gameObject.CompareTag("Ground"))
        {
            bc.enabled = false;
            Destroy(gameObject);
            NetworkObject.Despawn(gameObject);
        }
    }

}
