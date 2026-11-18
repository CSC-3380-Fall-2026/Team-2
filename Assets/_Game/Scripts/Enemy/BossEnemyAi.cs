
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossEnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float detectionRange = 12f;
    public float stopDistance = 4f;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootingRange = 8f;
    public float attackCooldown = 2f;

    [Header("Summoning")]
    public GameObject minionPrefab;
    public float spawnDistance = 2f;

    private Transform player;
    private EnemyHealth bossHealth;
    private Rigidbody2D rb;

    private float nextAttackTime;
    private bool phaseOneStarted = false;
    private bool phaseTwoStarted = false;

    private Vector2 moveDirection;

    void Start()
    {
        bossHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject target =
                GameObject.FindGameObjectWithTag("Player");

            if (target != null)
                player = target.transform;
        }

        if (bossHealth.CurrentHealth <= 15 && !phaseOneStarted)
        {
            phaseOneStarted = true;
            SpawnMinions(3);
        }

        if (bossHealth.CurrentHealth <= 10 && !phaseTwoStarted)
        {
            phaseTwoStarted = true;
            SpawnMinions(3);
            moveSpeed = 2.5f;
        }

        moveDirection = Vector2.zero;

        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > detectionRange)
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
        rb.MovePosition(
            rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime
        );
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        EnemyProjectile projectile =
            bullet.GetComponent<EnemyProjectile>();

        if (projectile != null)
            projectile.SetDirection(direction);
    }

    void SpawnMinions(int amount)
    {
        if (minionPrefab == null)
        {
            Debug.LogWarning("Boss minion prefab is missing!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            float angle = i * Mathf.PI * 2f / amount;

            Vector3 offset = new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0f
            ) * spawnDistance;

            Instantiate(
                minionPrefab,
                transform.position + offset,
                Quaternion.identity
            );
        }

        Debug.Log("Boss spawned " + amount + " enemies!");
    }
}
