using System;
using UnityEngine;
using Random = UnityEngine.Random;
using CalculatingSystem;

public class GunBase : MonoBehaviour
{
    [SerializeField] private ObjectList bulletPrefabs;
    private BulletPool bulletPool;
    private Context context;
    private WeaponManager manager;

    [SerializeField] private GunData data;
    public GunData Data => data;

    public bool IsReloading => manager.IsReloading;
    public event Action OnFire;


    public void Initialize(GunData data, Creature owner, WeaponManager wm)
    {
        this.data = data;
        context = new Context { Gun = this, Owner = owner };

        GetComponent<SpriteRenderer>().sprite = data.GunSprite;

        Transform spawn = transform.childCount > 0 ? transform.GetChild(0) : transform;
        bulletPool = new BulletPool(bulletPrefabs.Entries[(int)data.BulletType], spawn, data.BulletData, context);
        manager = wm;
    }

    public bool CanShoot() => data.CurrentAmmo != 0 && (data.fireTime <= Time.time || float.IsNaN(data.fireTime)) && data.GunType != GunType.Empty;
    public void Reload()
    {
        data.CurrentAmmo = data.MagazineSize;
        data.fireTime = Time.time;
    }

    public void Fire()
    {
        if (!CanShoot()) return;
        if (IsReloading) manager.StopReloading();

        float shootSpeed = data.FireRate.Evaluate(context);
        data.fireTime = shootSpeed != 0 ? Time.time + 1f / Mathf.Abs(shootSpeed) : float.NaN;

        float accuracy = data.Accuracy.Evaluate(context);
        float spread = accuracy == 0 ? 0 : 5 / accuracy;

        bulletPool.Get().Fire(Random.Range(-spread, spread));

        if (data.MagazineSize != 0) data.CurrentAmmo--;
        if (data.CurrentAmmo == 0) manager.Reload();
        OnFire?.Invoke();
    }
}


[Serializable]
public class GunData
{
    public string Name;

    [SerializeReference] public FormulaNode FireRate;
    [SerializeReference] public FormulaNode Accuracy;

    public int MagazineSize;
    public int CurrentAmmo;
    public float Echo;
    public float ReloadTime;
    public GunType GunType;
    public Sprite GunSprite;
    public float fireTime;

    public BulletType BulletType;
    public BulletData BulletData;

    public GunData(
        string name = "",
        float fireRate = 0,
        float accuracy = 1,
        int magazineSize = 0,
        float reload = 0,
        GunType gunType = GunType.Empty,
        BulletType bulletType = BulletType.Bullet,
        BulletData bulletData = null
    )
    {
        Name = name;
        FireRate = new ConstantNode(fireRate);
        Accuracy = new ConstantNode(accuracy);
        ReloadTime = reload;
        MagazineSize = magazineSize;
        CurrentAmmo = MagazineSize == 0 ? 1 : MagazineSize;
        GunType = gunType;
        BulletType = bulletType;
        BulletData = bulletData;
        GunSprite = WeaponSpriteGenerator.instance.RandomSprite();
        Echo = 0;
    }

    public GunData()
    {
        Name = "Empty";
        FireRate = new ConstantNode(0);
        Accuracy = new ConstantNode(1);
        ReloadTime = 0;
        MagazineSize = 0;
        CurrentAmmo = 1;
        GunType = GunType.Empty;
        BulletType = BulletType.Bullet;
        BulletData = null;
        GunSprite = null;
        Echo = 0;
    }
}


public enum GunType
{
    Empty,
    Pistol
}

public enum BulletType
{
    Bullet
}
