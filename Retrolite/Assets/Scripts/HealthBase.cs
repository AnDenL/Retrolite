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

    [SerializeField] protected int maxStability = 1;
    [SerializeField] protected int stability;
    public int Stability => stability;

    [SerializeReference] protected ConditionNode weakness;
    [SerializeReference] protected FormulaNode criticalDamage;

    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHeal;
    public event Action<float> OnDamaged;
    public event Action OnDeath;
    public event Action<int> OnStabilityChange;

    [HideInInspector] public Knockback knockback;

    protected virtual void Start()
    {
        health = maxHealth;
        stability = maxStability;
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

    public void TakeDamage(float damage, FormulaContext context)
    {
        if (weakness.Evaluate(context))
        {
            damage += criticalDamage.Evaluate(context);
            if (knockback) knockback.Multiplier = 3;
            ParticleManager.PlayParticle(3, transform.position);
        }
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

    public virtual void Corrupt(int strength)
    {
        stability -= strength;
        OnStabilityChange?.Invoke(stability);
    }

    public virtual float GetHealthPercent() => health / maxHealth;

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
    }
}
