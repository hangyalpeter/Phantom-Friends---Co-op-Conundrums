using System.Collections;
using UnityEngine;

public class RotateShootingBehavior : MonoBehaviour
{
    public GameObject projectilePrefab;

    private float spawnRadius = 1.0f;
    private float projectileSpeed = 10f;

    private float spawnInterval = 1f;

    // How fast the spawner rotates (degrees per second)
    private float rotationSpeed = 60f;

    public float angleStep = 30f; // Change this for more or fewer projectiles in a full circle

    private float currentAngle = 0f;

    public string collisionTag = "Enemy";

    public float damage = 25f;

    public Transform target { get; internal set; }

    private void Start()
    {
        GetComponent<SpriteRenderer>().flipX = true;
        StartCoroutine(SpawnProjectiles());
    }

    private IEnumerator SpawnProjectiles()
    {
        while (true)
        {
            SpawnProjectileInCircle();
            yield return new WaitForSeconds(spawnInterval); 
        }
    }

    private void SpawnProjectileInCircle()
    {
        float angleInRadians = currentAngle * Mathf.Deg2Rad;

        float angle = Time.time * rotationSpeed;
        Vector3 spawnDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;


        Vector3 spawnPoint = transform.position + spawnDirection * spawnRadius;

        ProjectileFactory.Instance.GetProjectile(projectilePrefab, spawnPoint, spawnDirection, projectileSpeed, damage, collisionTag);
        
        float angleInDegrees = Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);

        currentAngle += angleStep;

        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }
    }

}


