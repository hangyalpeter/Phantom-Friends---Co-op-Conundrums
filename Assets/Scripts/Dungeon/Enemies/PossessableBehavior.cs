using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

// TODO separate these behaviors even more or make a parent class which only updates the direction of movement (shooting direction) and descend from that
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

        bulletPrefab = Resources.Load<GameObject>("Trunk_Enemy_Bullet");

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
            ProjectileFactory.Instance.GetProjectile(bulletPrefab, transform.position + new Vector3(lastMovementDirection.x*1.75f, lastMovementDirection.y, 0), lastMovementDirection, 20, sb.damage , "PossessedEnemy");

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

    public void OnPossess()
    {
        gameObject.tag = "PossessedEnemy";
        pm.SetPossessedTrue();
        sb.enabled = false;
        followPlayerBehavior.enabled = false;
        isPossessed = true;
    }

    public void OnDePossess()
    {
        gameObject.tag = "Enemy";
        pm.SetPossessedFalse();
        
        sb.enabled = true;
        followPlayerBehavior.enabled = true;
        isPossessed = false;
    }

}
