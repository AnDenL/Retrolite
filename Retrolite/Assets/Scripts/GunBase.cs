using UnityEngine;
using CalculatingSystem;
using System.Collections.Generic;

public class GunBase : MonoBehaviour
{
    [SerializeField]
    public GunData data;
    [SerializeField]
    protected GameObject bulletPrefab;

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
        data = gun;
    }

    protected void Update()
    {
        if (data.GunType == GunType.Empty) return;
        if (Time.time >= fireTime && Input.GetButton("Fire1")) Fire();
    }

    protected void Fire()
    {
        if (!bullets[lastBulletIndex].Destroyed) CreateBullet();
        fireTime = Time.time + 1f / data.FireRate.Evaluate(context);
        bullets[lastBulletIndex].gameObject.SetActive(true);
        bullets[lastBulletIndex].Fire();
        lastBulletIndex++;
        if (lastBulletIndex >= bullets.Count) lastBulletIndex = 0;
    }

    protected void CreateBullet()
    {
        var bullet = Instantiate(bulletPrefab, transform).GetComponent<BulletBase>();
        bullet.Initialize(this, data.BulletData, context);
        bullets.Insert(lastBulletIndex, bullet);
        bullets[lastBulletIndex].gameObject.SetActive(false);
    }
}
[System.Serializable]
public struct GunData
{
    public FormulaNode FireRate;
    public float Accuracy;
    public int MagazineSize;
    public GunType GunType;

    public BulletData BulletData;

    public GunData(float fireRate = 0, float accuracy = 0, int magazineSize = 0, GunType gunType = GunType.Empty, BulletData bulletData = new BulletData())
    {
        FireRate = new ConstantNode(fireRate);
        Accuracy = accuracy;
        MagazineSize = magazineSize;
        GunType = gunType;
        BulletData = bulletData;
    }
}

public enum GunType
{
    Empty,
    Pistol,
    Shotgun,
    Rifle
}
