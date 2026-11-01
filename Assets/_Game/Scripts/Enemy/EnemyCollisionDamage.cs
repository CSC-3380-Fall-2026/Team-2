using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyCollisionDamage : MonoBehaviour
{
    public float minimumSpeed = 10f;
    public int damage = 1;
    public float hitDelay = 0.3f;
    private float nextHit;

    void OnCollisionEnter2D(Collision2D collision)
    {
        HitEnemy(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        HitEnemy(collision.gameObject);
    }

    void HitEnemy(GameObject other)
    {
        if (!other.CompareTag("Player") || Time.time < nextHit) return;
        Player player = other.GetComponent<Player>();
        if (player == null || player.rb == null) return;
        if (!player.isSpinning && !player.isLaunching) return;
        if (player.rb.linearVelocity.magnitude < minimumSpeed) return;

        GetComponent<EnemyHealth>().TakeDamage(damage);
        nextHit = Time.time + hitDelay;
    }
}
