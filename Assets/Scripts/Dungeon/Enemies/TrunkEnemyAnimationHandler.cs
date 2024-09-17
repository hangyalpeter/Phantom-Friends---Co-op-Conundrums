using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkEnemyAnimationHandler : MonoBehaviour
{
    private enum MovementState { idle, run}
    private Animator anim;
    private Vector3 lastPosition;
    private ShootBehavior shootBehavior;

    private HealthComponent healthComponent;

    MovementState currentState = MovementState.idle;
    void Start()
    {

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        shootBehavior = GetComponent<ShootBehavior>();
        healthComponent = GetComponent<HealthComponent>();

        if (shootBehavior != null)
        {
            shootBehavior.OnShoot += HandleShootAnimation;
        }

        if (healthComponent != null)
        {

            healthComponent.OnDamageTaken += PlayHitAnimation;
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
            healthComponent.OnDamageTaken -= PlayHitAnimation;
        }
    }

    private void HandleShootAnimation()
    {
        anim.SetTrigger("attack");

    }

    void Update()
    {


        if (!transform.position.Equals(lastPosition))
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

    private void PlayHitAnimation()
    {
        anim.SetTrigger("hit");
    }
}
