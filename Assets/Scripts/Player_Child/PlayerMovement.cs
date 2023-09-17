using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private float dirX = 0f;
    private float moveSpeed = 7f;
    private float jumpForce = 14f;
    
    private enum MovementState { idle, running, jumping, falling}
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        dirX= Input.GetAxisRaw("Horizontal_Child");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
       
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sr.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sr.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
 
    }
}
