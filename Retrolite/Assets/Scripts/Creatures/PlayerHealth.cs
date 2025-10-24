using UnityEngine;

public class PlayerHealth : HealthBase
{
    [SerializeField] float lives;

    [HideInInspector] public Creature Creature;

    private float invincibilityTimer;

    protected override void Start()
    {
        Creature = GetComponent<Creature>();

        TakeDamage(0);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }

    public override void TakeDamage(float damage)
    {
        if (invincibilityTimer > Time.time)
            return;

        base.TakeDamage(damage);

        invincibilityTimer = Time.time + 1f;
    }

    protected override void Die()
    {
        if (lives > 0) lives--;
        else base.Die();
    }
}