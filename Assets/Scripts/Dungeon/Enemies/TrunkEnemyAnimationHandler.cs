using UnityEngine;

public class TrunkEnemyAnimationHandler : MonoBehaviour
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

        if (shootBehavior != null)
        {
            shootBehavior.OnShoot += HandleShootAnimation;
        }

        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged += PlayHitAnimation;
        }

    }

    void OnDestroy()
    {

        if (shootBehavior != null)
        {
            shootBehavior.OnShoot -= HandleShootAnimation;
        }

        if ( healthComponent != null )
        {
            healthComponent.OnHealthChanged -= PlayHitAnimation;
        }
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

    private void PlayHitAnimation(float _)
    {
        anim.SetTrigger("hit");
    }
}
