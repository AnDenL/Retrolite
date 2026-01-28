using UnityEngine;
using TMPro;
using System.Text;

[RequireComponent(typeof(SpriteRenderer))]
public class GunPickUp : Interactable, IGenerationStruct
{
    [SerializeField] private GunData gunData;
    public TextMeshPro Description;

    protected override void Start()
    {
        base.Start();
        GetComponent<SpriteRenderer>().sprite = gunData.GunSprite;
        SetDescription();
    }

    public void Generate(GameRandom random)
    {
        gunData.Generate(random);
        GetComponent<SpriteRenderer>().sprite = gunData.GunSprite;
        SetDescription();
    }

    public void SetDescription()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"<b>{gunData.Name}</b>");
        sb.AppendLine($"Fire Rate : {gunData.FireRate.ToReadableString()}");
        sb.AppendLine($"Accuracy : {gunData.Accuracy.ToReadableString()}");
        sb.AppendLine($"Ammo : {gunData.CurrentAmmo}/{gunData.MagazineSize}");
        sb.AppendLine("— Bullet —");
        sb.AppendLine($"Dmg : {gunData.BulletData.Damage.ToReadableString()}");
        sb.AppendLine($"Spd : {gunData.BulletData.Speed.ToReadableString()}");
        sb.AppendLine($"Size : {gunData.BulletData.Scale.ToReadableString()}");
        sb.AppendLine($"Life : {gunData.BulletData.LifeTime.ToReadableString()}");

        if (gunData.BulletData.OnDamage != null)
            sb.AppendLine($"<color=red>On Hit:</color> {gunData.BulletData.OnDamage.ToReadableString()}");

        if (gunData.BulletData.OnReturn != null)
            sb.AppendLine($"<color=yellow>On End:</color> {gunData.BulletData.OnReturn.ToReadableString()}");

        Description.text = sb.ToString();
    }

    public override void Interact(Creature creature)
    {
        var wm = creature.transform.GetComponentInChildren<WeaponManager>();
        if (wm == null) return;
        
        wm.AddGun(gunData);
        Destroy(gameObject);
    }

    public override void Outline()
    {
        base.Outline();    
        Description.gameObject.SetActive(true);
    }

    public override void CancelOutline()
    {
        base.CancelOutline();
        Description.gameObject.SetActive(false);
    }
}
