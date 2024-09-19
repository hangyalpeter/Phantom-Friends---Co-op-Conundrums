using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PossessableBehavior : MonoBehaviour
{
    private Vector2 lastMovementDirection;
    private FollowPlayerBehavior followPlayerBehavior;
    private PossessableTransformation pt;
    private Rigidbody2D rb;

    private bool isPossessed = false;

    private Vector2 currentPosition;
    private Vector2 previousPosition;

    private GameObject bulletPrefab;

    private ShootBehavior sb;
    private PosessableMovement pm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pt = GetComponent<PossessableTransformation>();
        sb = GetComponent<ShootBehavior>(); 
        pm = GetComponent<PosessableMovement>();

        bulletPrefab = sb.projectilePrefab;
        followPlayerBehavior = GetComponent<FollowPlayerBehavior>();

        lastMovementDirection = Vector2.right;
        previousPosition = transform.position;

        PossessableTransformation.OnPossessEvent += OnPossess;
        PossessableTransformation.OnDePossessEvent += OnDePossess;
        bulletPrefab = Resources.Load<GameObject>("Trunk_Enemy_Bullet");
    }

    private void OnDisable()
    {
        PossessableTransformation.OnPossessEvent -= OnPossess;
        PossessableTransformation.OnDePossessEvent -= OnDePossess;
    }


    void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {

        UpdateShootingDirection();
        if (Input.GetKeyDown(KeyCode.K) && isPossessed)
        {
            GameObject bullet = Instantiate(bulletPrefab, (transform.position), transform.rotation);

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

    private void OnPossess()
    {
        pm.SetPossessedTrue();
        sb.enabled = false;
        followPlayerBehavior.enabled = false;
        isPossessed = true;
    }

    private void OnDePossess()
    {
        pm.SetPossessedFalse();
        sb.enabled = true;
        followPlayerBehavior.enabled = true;
        isPossessed = false;

    }

}
