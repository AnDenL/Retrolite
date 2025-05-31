using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [SerializeField] protected float _health;

    public float MaxHealth;

    protected bool _alive = true;

    protected void Start() => MaxHealth = _health;

    public virtual void SetHealth(float Value)
    {
        if(Value < 0) Heal(-Value);
        else Hit(Value);
    }

    protected virtual void Hit(float damage)
    {
        if(damage == 0) return;

        _health -= damage;

        if(_health <= 0) Death();
    }

    protected virtual void Heal(float healing)
    {
        if(healing == 0) return;
    
        _health += healing;

        if(_health > MaxHealth) _health = MaxHealth;
    }

    protected virtual void Death()
    {
        _alive = false;
    }

    public virtual float HealthPercent()
    {
        return _health/MaxHealth;
    }
}
