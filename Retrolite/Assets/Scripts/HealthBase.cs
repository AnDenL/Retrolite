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

    [SerializeField] protected Rule[] weaknesses;


    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnHeal;
    public event Action<float> OnDamaged;
    public event Action OnDeath;
    public event Action<int> OnStabilityChange;

    [HideInInspector] public Knockback Knockback;

    protected virtual void Start()
    {
        health = maxHealth;
        stability = maxStability;
        Knockback = GetComponent<Knockback>();
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
        if (stability != 0)
        foreach (Rule rule in weaknesses)
            rule.Check(context);
        
        else 
        foreach (Rule rule in weaknesses)
            rule.ExecuteAll(context);

        TakeDamage(damage);
    }

    public virtual void Corrupt(int strength)
    {
        stability -= strength;
        OnStabilityChange(stability);
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
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
    }
}
