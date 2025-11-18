using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using CalculatingSystem;

public class GunBase : MonoBehaviour
{
    [SerializeField] private ObjectList bulletPrefabs;
    private BulletPool bulletPool;
    private FormulaContext context;
    private Coroutine reloadRoutine;

    [SerializeField] private GunData data;
    public GunData Data => data;

    public bool IsReloading { get; private set; }
    public event Action OnFire;
    public event Action OnReloadStart;
    public event Action<float> OnReload;
    public event Action OnReloadEnd;

    public void Initialize(GunData data, Creature owner)
    {
        this.data = data;
        context = new FormulaContext { Gun = this, Owner = owner };

        GetComponent<SpriteRenderer>().sprite = data.GunSprite;

        Transform spawn = transform.childCount > 0 ? transform.GetChild(0) : transform;
        bulletPool = new BulletPool(bulletPrefabs.Entries[(int)data.BulletType], spawn, data.BulletData, context);
    }

    public bool CanShoot() => data.CurrentAmmo != 0 && (data.fireTime <= Time.time || float.IsNaN(data.fireTime)) && data.GunType != GunType.Empty;
    public void Reload() => reloadRoutine = StartCoroutine(ReloadCoroutine());

    public void Fire()
    {
        if (!CanShoot()) return;
        if (IsReloading || reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
        }

        float shootSpeed = data.FireRate.Evaluate(context);
        data.fireTime = shootSpeed != 0 ? Time.time + 1f / Mathf.Abs(shootSpeed) : float.NaN;

        float accuracy = data.Accuracy.Evaluate(context);
        float spread = accuracy == 0 ? 0 : 5 / accuracy;

        bulletPool.Get().Fire(Random.Range(-spread, spread));

        if (data.MagazineSize != 0) data.CurrentAmmo--;
        if (data.CurrentAmmo == 0) reloadRoutine = StartCoroutine(ReloadCoroutine());
        OnFire?.Invoke();
    }

    private void OnDisable()
    {
        if (reloadRoutine != null) StopCoroutine(reloadRoutine);
    }

    private IEnumerator ReloadCoroutine()
    {
        OnReloadStart?.Invoke();
        IsReloading = true;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / data.ReloadTime;
            OnReload?.Invoke(t);
            yield return null;
        }

        data.CurrentAmmo = data.MagazineSize;
        data.fireTime = 0;
        IsReloading = false;
        OnReloadEnd?.Invoke();
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
