using UnityEngine;
using CalculatingSystem;
using System;

public class HealthBase : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 100f;
    public float MaxHealth => maxHealth; 
    [SerializeField] protected float health;
    public float Health => health;
    [SerializeField] protected bool isDead;
    public bool IsDead => isDead;

    [SerializeField] protected ConditionNode weakness;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHeal;
    public event Action<float> OnDamaged;
    public event Action OnDeath;

    [HideInInspector] public Knockback knockback;


    protected virtual void Start()
    {
        health = maxHealth;
        knockback = GetComponent<Knockback>();
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
