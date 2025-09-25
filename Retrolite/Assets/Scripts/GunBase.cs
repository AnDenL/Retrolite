using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CalculatingSystem;

public class GunBase : MonoBehaviour
{
    [SerializeField] protected ObjectList bulletPrefabs;
    [SerializeField] protected Material reloadMaterial;
    [SerializeField] protected GameObject reloadBar;

    [SerializeField] public GunData Data;

    protected float fireTime;
    protected bool isReloading;
    protected FormulaContext context;

    private BulletPool bulletPool;

    public event Action OnFire;

    protected void Awake()
    {
        context = new FormulaContext();
        context.Gun = this;
        reloadBar.SetActive(false);
    }

    public void Set(GunData gun)
    {
        Data = gun;
        GetComponent<SpriteRenderer>().sprite = Data.GunSprite;

        bulletPool?.Clear();

        bulletPool = new BulletPool(bulletPrefabs.Entries[(int)Data.BulletType], transform.GetChild(0), Player.instance.Creature, Data.BulletData, context);
    }

    protected void Update()
    {
        if (Data.GunType == GunType.Empty) return;
        if (Time.time >= fireTime && Input.GetButton("Fire1")) Fire();
        else if (Input.GetKeyDown(KeyCode.R) && Data.CurrentAmmo != Data.MagazineSize && !isReloading) StartCoroutine(Reload());
    }

    protected void Fire()
    {
        if (Data.CurrentAmmo <= 0)
        {
            if (!isReloading) StartCoroutine(Reload());
            return;
        }

        float shootSpeed = Data.FireRate.Evaluate(context);

        if (shootSpeed != 0)
            fireTime = Time.time + 1f / Mathf.Abs(shootSpeed);
        else fireTime = float.NaN;

        float Spread = 5 / Data.Accuracy.Evaluate(context);

        bulletPool.Get().Fire(Spread);

        isReloading = false;
        if (Data.MagazineSize != 0) Data.CurrentAmmo -= 1;
        OnFire?.Invoke();
    }

    protected IEnumerator Reload()
    {
        float t = 0;
        isReloading = true;
        reloadBar.SetActive(true);

        while (isReloading)
        {
            t += Time.deltaTime / Data.ReloadTime;
            reloadMaterial.SetFloat("_Fill", t);
            if (t > 1) isReloading = false;
            yield return null;
        }

        if (t > 1)
        {
            Data.CurrentAmmo = Data.MagazineSize;
        }
        reloadBar.SetActive(false);
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
    public Sprite BulletSprite;

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
        BulletSprite = WeaponSpriteGenerator.instance.BulletList.RandomSprite();
        Echo = 0;
    }
}


public enum GunType
{
    Empty,
    Pistol,
    Shotgun,
    Rifle
}

public enum BulletType
{
    Bullet,
    Electric,
    Sound,
    Laser,
    Explosive,
    Poison
}
