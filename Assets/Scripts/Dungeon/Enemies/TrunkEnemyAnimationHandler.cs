using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkEnemyAnimationHandler : MonoBehaviour
{
    private enum MovementState { idle, run, hit, attack }
    private Animator anim;
    private Vector3 lastPosition;
    private ShootBehavior shootBehavior;

    MovementState state = MovementState.idle;
    void Start()
    {

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        shootBehavior = GetComponent<ShootBehavior>();

        if (shootBehavior != null)
        {
            shootBehavior.OnShoot += HandleShoot;
        }

    }

    void OnDestroy()
    {
        if (shootBehavior != null)
        {
            shootBehavior.OnShoot -= HandleShoot;
        }
    }

    private void HandleShoot()
    {
        state = MovementState.attack;

    }

    void Update()
    {


        if (transform.position != lastPosition)
        {
            state = MovementState.run;
            
        } else
        {
            state = MovementState.idle;
        }

        
        anim.SetInteger("state", (int)state);
    }

    // TODO: if have time check these animations
    public void PlayHitAnimation()
    {
        state = MovementState.hit;
    }
}
