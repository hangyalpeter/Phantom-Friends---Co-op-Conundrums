using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

// TODO separate these behaviors even more
public class PossessableBehavior : MonoBehaviour
{
    private Vector2 lastMovementDirection;
    private FollowPlayerBehavior followPlayerBehavior;

    private bool isPossessed = false;

    private Vector2 currentPosition;
    private Vector2 previousPosition;

    private GameObject bulletPrefab;

    private ShootBehavior sb;
    private PosessableMovement pm;


    public bool IsPossessed => isPossessed;

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.K) && isPossessed && gameObject.tag == "PossessedEnemy")
        {
            ProjectileFactory.Instance.GetProjectile(bulletPrefab, transform.position + new Vector3(lastMovementDirection.x*1.75f, lastMovementDirection.y, 0), lastMovementDirection, 20, "PossessedEnemy");

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

        Debug.Log("Direction: " + lastMovementDirection);

        previousPosition = currentPosition;
    }

    private void OnPossess()
    {
        gameObject.tag = "PossessedEnemy";
        pm.SetPossessedTrue();
        sb.enabled = false;
        followPlayerBehavior.enabled = false;
        isPossessed = true;
    }

    private void OnDePossess()
    {
        gameObject.tag = "Enemy";
        pm.SetPossessedFalse();
        
        sb.enabled = true;
        followPlayerBehavior.enabled = true;
        isPossessed = false;
    }

}
