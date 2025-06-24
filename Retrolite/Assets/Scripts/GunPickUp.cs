using UnityEngine;

public class GunPickUp : Interactable
{
    [SerializeField]
    private GunData gunData;

    public override void Interact(Player player)
    {
        player.SetGun(gunData);
    }
}
