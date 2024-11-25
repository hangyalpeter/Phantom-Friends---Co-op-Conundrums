using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementDungeon : NetworkBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator anim;
    private HealthBase health;

    private ProjectileSpawner projectileSpawner;

    private NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float dirX = 0f;
    private float dirY = 0f;
    private float moveSpeed = 7f;

    public float Damage = 25f;

    private Vector2 previousPosition;
    private Vector2 currentPosition;
    private Vector2 lastMovementDirection;
    private enum MovementState { idle, running, jumping, falling }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isFlipped.OnValueChanged += (oldValue, newValue) =>
            {
                sr.flipX = newValue;
            };
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<HealthBase>();
        projectileSpawner = GetComponent<ProjectileSpawner>();

        previousPosition = transform.position;
        lastMovementDirection = Vector2.right;

        health.OnHealthChanged += TriggerHitAnimation;
        health.OnDied += OnDied;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= TriggerHitAnimation;
        health.OnDied -= OnDied;
    }

    private void OnDied()
    {
        anim.SetTrigger("death");
    }

    void Update()
    {
        if (!IsOwner) return;
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
            projectileSpawner.GetProjectile(transform.position, lastMovementDirection, 20, Damage, "Player_Child");
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
            UpdateFlipX(false);
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            UpdateFlipX(true);
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

    private void TriggerHitAnimation(float _) {
        anim.SetTrigger("hit");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Ghost"))
        {
            Physics2D.IgnoreCollision(collision.collider, bc);
        }
    }   
}
