using UnityEngine;
using System;

public class HealthBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    protected float maxHealth = 100f;
    [SerializeField]
    protected float health;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHeal;
    public event Action<float> OnDamaged;
    public event Action OnDeath;

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
        OnHeal?.Invoke(amount);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health > maxHealth)
            health = maxHealth;
        else if (health <= 0)
            Die();
        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public virtual float GetHealthPercent() => health / maxHealth;

    protected virtual void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
