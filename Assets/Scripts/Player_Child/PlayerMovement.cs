using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator anim;
    private float dirX = 0f;
    private float moveSpeed = 7f;
    private float jumpForce = 14f;

    [SerializeField] private AudioSource jumpSound;

    private NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isSpriteEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool jumpRequested = false;

    private enum MovementState { idle, running, jumping, falling}
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        isFlipped.OnValueChanged += (oldValue, newValue) =>
        {
            sr.flipX = newValue;
        };

        isSpriteEnabled.OnValueChanged += (oldValue, newValue) =>
        {
            sr.enabled = newValue;
        };

    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump") && IsChildGrounded())
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        if (float.Equals(Time.timeScale, 0f))
        {
            return;
        }
        dirX= Input.GetAxisRaw("Horizontal_Child");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        if (jumpRequested)
        {
            PlayJumpSoundServerRpc();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequested = false;
        }
       
        UpdateAnimationState();
    }

    [ServerRpc]
    private void PlayJumpSoundServerRpc()
    {
        PlayJumpSoundClientRpc();

    }

    [ClientRpc]
    private void PlayJumpSoundClientRpc()
    {
        jumpSound.Play();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            UpdateFlipX(false);
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sr.flipX = true;
            UpdateFlipX(true);
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

     private void UpdateFlipX(bool flipX)
    {
        if (IsServer)
        {
            isFlipped.Value = flipX;
        }
        else
        {
            UpdateFlipXServerRpc(flipX);
        }
    }

       [ServerRpc]
    private void UpdateFlipXServerRpc(bool flipX)
    {
        isFlipped.Value = flipX;
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
