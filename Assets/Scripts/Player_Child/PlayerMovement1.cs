using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement1 : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator anim;
    private float dirX = 0f;
    private float dirY= 0f;
    private float moveSpeed = 7f;

    [SerializeField] private AudioSource jumpSound;
    private GameObject bulletPrefab;

    private Vector2 previousPosition;
    private Vector2 currentPosition;
    private Vector2 lastMovementDirection;
    private enum MovementState { idle, running, jumping, falling }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        bulletPrefab = Resources.Load<GameObject>("PlayerBullet");
        if (bulletPrefab == null)
        {
            Debug.LogError("Couldn't load bullet asset");
        }
        previousPosition = transform.position;
        lastMovementDirection = Vector2.right;

    }

    void Update()
    {
        if (float.Equals(Time.timeScale, 0f))
        {
            return;
        }
        dirX = Input.GetAxisRaw("Horizontal_Child");
        dirY = Input.GetAxisRaw("Vertical_Child");
        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);

        UpdateAnimationState();

        HandleShooting();

    }

    private void HandleShooting()
    {
        UpdateShootingDirection();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            bullet.GetComponent<Rigidbody2D>().velocity = lastMovementDirection * 20f;
        }
    }

    private void UpdateShootingDirection()
    {
        currentPosition = transform.position;
        var direction = currentPosition - previousPosition;

        if (direction != Vector2.zero)
        {
            lastMovementDirection = direction.normalized;
        }


        previousPosition = currentPosition;
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
            state = MovementState.running;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.running;
        }

        anim.SetInteger("state", (int)state);
 
    }

    private bool IsChildGrounded()
    {
        return Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, LayerMask.GetMask("Ground"));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Ghost"))
        {
            Physics2D.IgnoreCollision(collision.collider, bc);
        }
    }   
}
