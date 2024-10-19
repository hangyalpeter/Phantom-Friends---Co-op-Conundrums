using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelFinish : NetworkBehaviour
{
    [SerializeField] private AudioSource levelFinishSound;
    
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            LevelFinishServerRpc();
        }

    }

    private void LevelFinishActions()
    {
        levelFinishSound.Play();
        GameEvents.LevelFinished?.Invoke();
        UIScreenEvents.LevelFinishShown?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LevelFinishServerRpc()
    {
        LevelFinishClientRpc();
    }

    [ClientRpc]
    private void LevelFinishClientRpc()
    {
        LevelFinishActions();
    }
}
