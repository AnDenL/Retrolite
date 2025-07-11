using UnityEngine;

public class GunPickUp : Interactable
{
    [SerializeField]
    private GunData gunData;

    [SerializeField]
    private Sprite part1, part2;
    [SerializeField] [Range(0.0f, 1.0f)]
    private float hueShift;

    private void Start() => SetSprite();

    [ContextMenu("SetSprite")]
    private void SetSprite()
    {
        gunData.GunSprite = WeaponSpriteGenerator.CombineSprites(part1, part2, hueShift);
        GetComponent<SpriteRenderer>().sprite = gunData.GunSprite;
    }

    public override void Interact(Player player)
    {
        gunData = player.SetGun(gunData);

        if (gunData.GunType == GunType.Empty)
            Destroy(gameObject);
    }
}
