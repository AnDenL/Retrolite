using UnityEngine;
using CalculatingSystem;
using System;

public class HealthBase : MonoBehaviour, IDamagable
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    [SerializeField] protected float health;
    public float Health => health;

    public float HealthEditable
    {
        get => health;
        set
        {
            health = value;
            OnHealthChanged?.Invoke(health, maxHealth);
            if (value <= 0) Die();
        }
    }

    [SerializeField] protected bool isDead;
    public bool IsDead => isDead;

    public bool IsWeak;

    [SerializeField] protected float destroyDelay = 2f;

    [SerializeField] protected Rule[] weaknesses;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHeal;
    public event Action<float> OnDamaged;
    public event Action<float, Context> ContextDamaged;
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

    public virtual void TakeDamage(float damage, Context context)
    {
        if (!IsWeak)
            foreach (Rule rule in weaknesses)
                rule.Check(context);
        else
            foreach (Rule rule in weaknesses)
                rule.ExecuteAll(context);

        ContextDamaged?.Invoke(damage, context);

        TakeDamage(damage);
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

    public virtual void Knockback(Vector2 dir, float strength) {}

    public virtual void AddMaximumHealth(float amount)
    {
        if (amount <= 0)
            return;

        maxHealth += amount;
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public virtual void SetHealth(float Health, float Maxhealth)
    {
        health = Health;
        maxHealth = MaxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public virtual float GetHealthPercent() => health / maxHealth;

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject, destroyDelay);
    }
}
