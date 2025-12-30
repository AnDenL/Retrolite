using UnityEngine;

public class Robot : Creature
{
    [Header("Parts")]
    [SerializeField] private Transform UI;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftTurbine;
    [SerializeField] private Transform rightTurbine;

    private Vector2 headPosition;
    private Vector2 leftTurbinePosition;
    private Vector2 rightTurbinePosition;

    private void Start()
    {
        HealthComponent.OnDamaged += Damage;
        headPosition = head.localPosition;
        leftTurbinePosition = leftTurbine.localPosition;
        rightTurbinePosition = rightTurbine.localPosition;
    }

    private void Damage(float t)
    {
        ParticleManager.PlayParticle("RobotDetails", transform.position);

        headPosition += 0.005f * t * Random.insideUnitCircle;
        leftTurbinePosition += 0.005f * t * Random.insideUnitCircle;
        rightTurbinePosition += 0.005f * t * Random.insideUnitCircle;
    }

    protected override void Update()
    {
        base.Update();
        if (HealthComponent.IsDead) return;
        AnimateParts();
    }

    private void AnimateParts()
    {
        float strength = 1 - HealthComponent.GetHealthPercent();

        head.localPosition = headPosition + 0.1f * strength * Random.insideUnitCircle;
        leftTurbine.localPosition = leftTurbinePosition + 0.1f * strength * Random.insideUnitCircle;
        rightTurbine.localPosition = rightTurbinePosition + 0.1f * strength * Random.insideUnitCircle;
    }

    public void SelfDestraction(Creature other)
    {
        Vector2 position = transform.position;
        ParticleManager.PlayParticle("Explosion", position);
        var hits = Physics2D.OverlapCircleAll(position, 5, LayerMask.GetMask("Creature"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Creature creature))
            {
                if (!creature.IsEnemyTo(other) || creature == this) continue;
                creature.HealthComponent.TakeDamage(HealthComponent.Health);
                Vector2 dir = hit.transform.position - (Vector3)position;
                creature.Rb.AddForce(15 * dir, ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);

        ParticleManager.PlayParticle("RobotDetails", transform.position);
    }
}
