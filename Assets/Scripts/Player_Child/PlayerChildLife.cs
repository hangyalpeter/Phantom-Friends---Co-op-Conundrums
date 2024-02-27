using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerChildLife : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private AudioSource deathSound;
    void Start()
    {
       animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        deathSound.Play();
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("death");
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameEvents.OnLevelRestart?.Invoke();

    }

}
