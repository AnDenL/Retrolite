using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CalculatingSystem;

public class GunBase : MonoBehaviour
{
    [SerializeField]
    protected BulletRegistry bulletPrefabs;
    [SerializeField]
    protected Material reloadMaterial;
    [SerializeField]
    protected GameObject reloadBar;

    [SerializeField]
    public GunData Data;

    protected float fireTime;
    protected List<BulletBase> bullets;
    protected int lastBulletIndex;
    protected bool isReloading;
    protected FormulaContext context;

    protected void Awake()
    {
        context = new FormulaContext();
        bullets = new List<BulletBase>();
        CreateBullet();
        lastBulletIndex = 0;
        reloadBar.SetActive(false);
    }

    public void Set(GunData gun)
    {
        Data = gun;
        GetComponent<SpriteRenderer>().sprite = Data.GunSprite;
    }

    protected void Update()
    {
        if (Data.GunType == GunType.Empty) return;
        if (Time.time >= fireTime && Input.GetButton("Fire1")) Fire();
        else if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());
    }

    protected void Fire()
    {
        if (Data.CurrentAmmo <= 0)
        {
            if (!isReloading) StartCoroutine(Reload());
            return;
        }
        if (!bullets[lastBulletIndex].Destroyed) CreateBullet();

        float shootSpeed = Data.FireRate.Evaluate(context);

        if (shootSpeed != 0)
            fireTime = Time.time + 1f / Mathf.Abs(shootSpeed);
        else fireTime = float.NaN;

        float Spread = 5 / Data.Accuracy.Evaluate(context);

        bullets[lastBulletIndex].gameObject.SetActive(true);
        bullets[lastBulletIndex].Fire(Random.Range(-Spread, Spread));
        lastBulletIndex++;
        isReloading = false;
        if (Data.MagazineSize != 0) Data.CurrentAmmo -= 1;

        if (lastBulletIndex >= bullets.Count) lastBulletIndex = 0;
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

    protected void CreateBullet()
    {
        var bullet = Instantiate(bulletPrefabs.Entries[(int)Data.BulletType], transform.GetChild(0).transform).GetComponent<BulletBase>();

        bullet.Initialize(this, Data.BulletData, context);
        bullets.Insert(lastBulletIndex, bullet);
        bullets[lastBulletIndex].gameObject.SetActive(false);
    }
}
[Serializable]
public struct GunData
{
    [SerializeReference]
    public FormulaNode FireRate;
    [SerializeReference]
    public FormulaNode Accuracy;

    public int MagazineSize;
    public int CurrentAmmo;
    public float Echo;
    public float ReloadTime;    
    public GunType GunType;
    public Sprite GunSprite;

    public BulletType BulletType;
    public BulletData BulletData;

    public GunData(float fireRate = 0, float accuracy = 1, int magazineSize = 0, float reload = 0, GunType gunType = GunType.Empty, BulletType bulletType = BulletType.Bullet, BulletData bulletData = null)
    {
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
}

[CreateAssetMenu(fileName = "BulletRegistry", menuName = "Game/BulletRegistry")]
public class BulletRegistry : ScriptableObject
{
    public GameObject[] Entries;
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
