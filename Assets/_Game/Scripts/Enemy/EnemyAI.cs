using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 7f;
    public float stopChasingRange = 10f;
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float attackDelay = 1.2f;
    public Transform[] patrolPoints;
    public LayerMask wallLayers;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 startPosition;
    private Vector2 moveDirection;
    private int patrolIndex;
    private float nextAttack;
    private bool chasing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }

        Vector2 destination = startPosition;
        float distance = Mathf.Infinity;

        if (player != null)
        {
            distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectionRange && CanSeePlayer()) chasing = true;

            if (distance > Mathf.Max(detectionRange, stopChasingRange) || !CanSeePlayer()) chasing = false;
        }
        else chasing = false;

        if (chasing && player != null)
        {
            destination = player.position;
            if (distance <= attackRange)
            {
                moveDirection = Vector2.zero;
                if (Time.time >= nextAttack)
                {
                    PlayerHealth health = player.GetComponent<PlayerHealth>();

                    

                    if (health != null) 
                        health.TakeDamage(attackDamage);
                    nextAttack = Time.time + attackDelay;
                }
                return;
            }
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            // Go home before starting the patrol again.
            if (Vector2.Distance(transform.position, startPosition) > 0.3f)
                destination = startPosition;
            else
            {
                Transform point = patrolPoints[patrolIndex];
                if (point != null)
                {
                    destination = point.position;
                    if (Vector2.Distance(transform.position, destination) < 0.3f)
                        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                }
            }
        }

        Vector2 difference = destination - (Vector2)transform.position;
        moveDirection = difference.magnitude > 0.2f ? difference.normalized : Vector2.zero;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector2 direction = player.position - transform.position;
        RaycastHit2D wall = Physics2D.Raycast(transform.position, direction.normalized,
            direction.magnitude, wallLayers);
        return wall.collider == null;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void OnDisable()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
