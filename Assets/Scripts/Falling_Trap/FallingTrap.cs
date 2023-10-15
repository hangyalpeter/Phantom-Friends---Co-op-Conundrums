using System.Collections;
using UnityEngine;

public class FallingTrap : MonoBehaviour
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

    private IEnumerator DestroyFallingTrap()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
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
            StartCoroutine(DestroyFallingTrap());
        }
    }
}
