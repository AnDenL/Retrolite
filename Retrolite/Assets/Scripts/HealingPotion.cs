using UnityEngine;

public class HealingPotion : Interactable
{
    [SerializeField] float healAmount = 10f;
    [SerializeField] float additionalHeal;

    public override void Interact(Player player)
    {
        player.HealthComponent.Heal(healAmount);
        if (additionalHeal != 0) player.HealthComponent.AddMaximumHealth(additionalHeal);
        Destroy(gameObject);
    }
}
