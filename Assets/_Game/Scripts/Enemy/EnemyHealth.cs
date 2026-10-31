using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    [SerializeField] private int health;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || health <= 0) return;
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }
}
