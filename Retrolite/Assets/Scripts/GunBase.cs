using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CalculatingSystem;

public class GunBase : MonoBehaviour
{
    [SerializeField]
    public GunData Data;
    [SerializeField]
    protected BulletRegistry bulletPrefabs;

    protected float fireTime;
    protected List<BulletBase> bullets;
    protected int lastBulletIndex;
    protected FormulaContext context;

    protected void Awake()
    {
        context = new FormulaContext();
        bullets = new List<BulletBase>();
        CreateBullet();
        lastBulletIndex = 0;
    }

    public void Set(GunData gun)
    {
        Data = gun;
    }

    protected void Update()
    {
        if (Data.GunType == GunType.Empty) return;
        if (Time.time >= fireTime && Input.GetButton("Fire1")) Fire();
    }

    protected void Fire()
    {
        if (Data.CurrentAmmo <= 0) return;
        if (!bullets[lastBulletIndex].Destroyed) CreateBullet();

        fireTime = Time.time + 1f / Data.FireRate.Evaluate(context);

        float Spread = 5 / Data.Accuracy.Evaluate(context);

        bullets[lastBulletIndex].gameObject.SetActive(true);
        bullets[lastBulletIndex].Fire(Random.Range(-Spread, Spread));
        lastBulletIndex++;
        if (Data.MagazineSize != 0) Data.CurrentAmmo -= 1;

        if (lastBulletIndex >= bullets.Count) lastBulletIndex = 0;
    }

    protected IEnumerator Reload()
    {
        yield return new WaitForSeconds(1f);
        Data.CurrentAmmo = Data.MagazineSize;
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
    public GunType GunType;

    public BulletType BulletType;
    public BulletData BulletData;

    public GunData(float fireRate = 0, float accuracy = 1, int magazineSize = 0, GunType gunType = GunType.Empty, BulletType bulletType = BulletType.Bullet, BulletData bulletData = new BulletData())
    {
        FireRate = new ConstantNode(fireRate);
        Accuracy = new ConstantNode(accuracy);
        MagazineSize = magazineSize;
        CurrentAmmo = MagazineSize == 0 ? 1 : MagazineSize;
        GunType = gunType;
        BulletType = bulletType;
        BulletData = bulletData;
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
