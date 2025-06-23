using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    protected float maxHealth = 100f;
    [SerializeField]
    protected float health;

    protected virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void Heal(float amount)
    {
        if (amount <= 0)
            return;

        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health > maxHealth)
            health = maxHealth;
        else if (health <= 0)
            Die();
    }

    public virtual float GetHealthPercent() => health / maxHealth;

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
