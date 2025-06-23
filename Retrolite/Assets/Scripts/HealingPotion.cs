using UnityEngine;

public class HealingPotion : Interactable
{
    [SerializeField]
    private float healAmount = 10f;
    [SerializeField]
    private float additionalHeal;

    public override void Interact(Player player)
    {
        player.Heal(healAmount);
        if (additionalHeal != 0) player.AddHealth(additionalHeal);
        Destroy(gameObject);
    }
}
