using UnityEngine;

public class HealingPotion : Interactable
{
    [SerializeField] float healAmount = 10f;
    [SerializeField] float additionalHeal;

    public override void Interact(Creature creature)
    {
        creature.HealthComponent.Heal(healAmount);
        if (additionalHeal != 0) creature.HealthComponent.AddMaximumHealth(additionalHeal);
        Destroy(gameObject);
    }
}
