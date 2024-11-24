using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerChildLife : NetworkBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private AudioSource deathSound;
    public static event Action OnPlayerChildDeath;
    void Start()
    {
       animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOwner) return;
       if (collision.gameObject.CompareTag("Trap"))
        {
            DieServerRpc();
        }
    }

    private IEnumerator DelayDeathScreenShow()
    {
        yield return new WaitForSeconds(0.5f);
        OnPlayerChildDeath?.Invoke();
    }

    private void Die()
    {
        deathSound.Play();
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("death");
        StartCoroutine(DelayDeathScreenShow());
    }

    [ServerRpc]
    private void DieServerRpc()
    {
       DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Die();
    }



    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameEvents.OnLevelRestart?.Invoke();

    }

}
