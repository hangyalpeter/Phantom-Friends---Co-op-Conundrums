using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleApple : MonoBehaviour
{

    public static event Action OnCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Child"))
        {
            OnCollected?.Invoke();
            Destroy(gameObject);
        }
    }
   
}
