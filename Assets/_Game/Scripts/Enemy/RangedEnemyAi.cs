
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]


[RequireComponent(typeof(EnemyHealth))]
public class RangedEnemyAI : MonoBehaviour
{
    public float moveSpeed = 2.5f;



    public float detectionRange = 8f;
    public float shootingRange = 5f;
    public float stopDistance = 3f;

    public float attackCooldown = 1.5f;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public LayerMask wallLayers;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDirection;
    private float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");

            if (target != null) player = target.transform;
        }

        moveDirection = Vector2.zero;

        if (player == null)
            return;

        float distance = Vector2.Distance( transform.position, player.position);

        if (distance > detectionRange || !CanSeePlayer())
            return;

        Vector2 direction =
            (player.position - transform.position).normalized;

        if (distance > stopDistance)
            moveDirection = direction;

        if (distance <= shootingRange &&
            Time.time >= nextAttackTime)
        {
            Shoot(direction);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    bool CanSeePlayer()
    {
        Vector2 direction = player.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            direction.magnitude,
            wallLayers);

        return hit.collider == null;
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null)
            return;

        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity);

        EnemyProjectile bullet =  projectile.GetComponent<EnemyProjectile>();

        if (bullet != null) bullet.SetDirection(direction);
    }
}
