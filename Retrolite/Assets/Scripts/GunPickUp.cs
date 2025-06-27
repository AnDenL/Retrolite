using UnityEngine;

public class GunPickUp : Interactable
{
    [SerializeField]
    private GunData gunData;

    public override void Interact(Player player)
    {
        gunData = player.SetGun(gunData);

        if (gunData.GunType == GunType.Empty)
            Destroy(gameObject);
    }
}
