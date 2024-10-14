using UnityEngine;

public class TrampolineJump : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private AudioSource trampolineSound;
    private Rigidbody2D rb;
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = playerGameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player_Child")
        {
            trampolineSound.Play();
            anim.SetTrigger("jump");
            JumpPlayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player_Child")
        {
            trampolineSound.Play();
            anim.SetTrigger("jump");
            JumpPlayer();
        }

    }

    public void JumpPlayer()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
   
}
