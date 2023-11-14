using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int appplesCount = 0;
    [SerializeField] private TextMeshProUGUI applesCountText;
    [SerializeField] private AudioSource appleSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Apple"))
        {
            appleSound.Play();
            Destroy(collision.gameObject);
            appplesCount++;
            applesCountText.text = "Apples: " + appplesCount;
        }
    }
}

