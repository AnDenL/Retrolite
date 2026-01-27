using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GunPickUp : Interactable, IGenerationStruct
{
    [SerializeField] private GunData gunData;

    [SerializeField][Range(0.0f, 1.0f)] private float hueShift;

    public void Generate(GameRandom random)
    {
        gunData.Generate(random);
        GetComponent<SpriteRenderer>().sprite = gunData.GunSprite;
    }

    public override void Interact(Creature creature)
    {
        var wm = creature.transform.GetComponentInChildren<WeaponManager>();
        if (wm == null) return;
        
        wm.AddGun(gunData);
        Destroy(gameObject);
    }
}
