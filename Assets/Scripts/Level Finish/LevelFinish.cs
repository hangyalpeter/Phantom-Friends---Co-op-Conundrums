using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] private AudioSource levelFinishSound;
    
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player_Child")
        {
            levelFinishSound.Play();
            GameEvents.LevelFinished?.Invoke();
            UIScreenEvents.LevelFinishShown?.Invoke();

        }

    }
}
