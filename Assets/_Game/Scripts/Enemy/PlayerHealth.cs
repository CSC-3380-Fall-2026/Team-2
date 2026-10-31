using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float damageCooldown = 0.6f;
    [SerializeField] private int health;
    private float nextDamageTime;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || health <= 0 || Time.time < nextDamageTime) return;
        health -= damage;
        nextDamageTime = Time.time + damageCooldown;
        Debug.Log("Player health: " + health);
        if (health <= 0) Debug.Log("Player defeated");
    }
}
