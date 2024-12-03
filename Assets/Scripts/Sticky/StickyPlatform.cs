using Unity.Netcode;
using UnityEngine;

public class StickyPlatform : NetworkBehaviour
{
    private bool isOnPlatform = false;
    private GameObject player;
    private Vector3 platformOffset;
    private Vector3 previousPlatformPosition;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            player = collision.gameObject;
            platformOffset = player.transform.position - transform.position;
            previousPlatformPosition = transform.position;

            isOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            player = null;
            isOnPlatform = false;
        }
    }

    private void LateUpdate()
    {
        if (player != null && isOnPlatform)
        {
            Vector3 platformMovement = transform.position - previousPlatformPosition;

            player.transform.position += platformMovement;

            previousPlatformPosition = transform.position;
        }
    }
}
