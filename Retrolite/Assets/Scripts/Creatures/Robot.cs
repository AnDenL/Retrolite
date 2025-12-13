using System.Collections;
using CalculatingSystem;
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
}
