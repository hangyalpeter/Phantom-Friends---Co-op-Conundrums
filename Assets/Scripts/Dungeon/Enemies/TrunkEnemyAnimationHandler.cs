using Unity.Netcode;
using UnityEngine;

public class TrunkEnemyAnimationHandler : NetworkBehaviour
{
    private enum MovementState { idle, run}
    private Animator anim;
    private Vector3 lastPosition;
    private ShootBehavior shootBehavior;

    private HealthComponent healthComponent;
    private Rigidbody2D rb;

    MovementState currentState = MovementState.idle;
    void Start()
    {

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        shootBehavior = GetComponent<ShootBehavior>();
        healthComponent = GetComponent<HealthComponent>();
        rb = GetComponent<Rigidbody2D>();

        if (shootBehavior != null && IsServer)
        {
            shootBehavior.OnShoot += HandleShootinAnimationClientRpc;
        }

        if (healthComponent != null && IsServer)
        {
            healthComponent.OnHealthChanged += PlayHitAnimationClientRpc;
        }

    }

    private void OnDisable()
    {

        if (shootBehavior != null && IsServer)
        {
            shootBehavior.OnShoot -= HandleShootinAnimationClientRpc;
        }

        if (healthComponent != null && IsServer)
        {
            healthComponent.OnHealthChanged -= PlayHitAnimationClientRpc;
        }
    }

    [ClientRpc]
    private void HandleShootinAnimationClientRpc()
    {
        HandleShootAnimation();
    }

    private void HandleShootAnimation()
    {
        anim.SetTrigger("attack");
    }


    void Update()
    {
        if (!transform.position.Equals(lastPosition) || rb.velocity.magnitude > 0 )
        {
            currentState = MovementState.run;

        }
        else
        {
            currentState = MovementState.idle;
        }


        anim.SetInteger("state", (int)currentState);
        lastPosition = transform.position;
    }

    [ClientRpc]
    private void PlayHitAnimationClientRpc(float _)
    {
        PlayHitAnimation(0);
    }

    private void PlayHitAnimation(float _)
    {
        anim.SetTrigger("hit");
    }
}
