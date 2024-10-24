using Unity.Netcode;
using UnityEngine;

// TODO remove class in the future
public class PossessableBehavior : NetworkBehaviour
{
    private Vector2 lastMovementDirection;
    private FollowPlayerBehavior followPlayerBehavior;

    private bool isPossessed = false;

    private Vector2 currentPosition;
    private Vector2 previousPosition;

    private GameObject bulletPrefab;
    

    private ShootBehavior sb;
    private PosessableMovement pm;

    private ProjectileSpawner projectileSpawner;

    public bool IsPossessed => isPossessed;

    void Start()
    {
        projectileSpawner = GetComponent<ProjectileSpawner>();
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
        if (!IsOwner) return;
        HandleShooting();
    }

    private void HandleShooting()
    {

        UpdateShootingDirection();
        if (Input.GetKeyDown(KeyCode.K) && isPossessed && gameObject.tag == "PossessedEnemy")
        {
            projectileSpawner.GetProjectile(transform.position, lastMovementDirection, 20, sb.damage, "PossessedEnemy");
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
        sb.enabled = false;
        followPlayerBehavior.enabled = false;
        isPossessed = true;
    }

    public void OnDePossess()
    {
        gameObject.tag = "Enemy";
        
        sb.enabled = true;
        followPlayerBehavior.enabled = true;
        isPossessed = false;
    }

}
