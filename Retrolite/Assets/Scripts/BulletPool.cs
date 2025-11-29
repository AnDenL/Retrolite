using System.Collections.Generic;
using UnityEngine;
using CalculatingSystem;
using System;
using Object = UnityEngine.Object;

public class BulletPool
{
    private Queue<BulletBase> freeBullets = new Queue<BulletBase>();
    private List<BulletBase> allBullets = new List<BulletBase>();

    private GameObject prefab;
    private Transform parent;
    private BulletData bulletData;
    private FormulaContext context;

    public event Action<BulletBase> OnBulletReturn;

    public BulletPool(GameObject prefab, Transform parent, BulletData bulletData, FormulaContext context)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.bulletData = bulletData;
        this.context = context;
    }

    public BulletBase Get()
    {
        if (freeBullets.Count > 0)
        {
            var bullet = freeBullets.Dequeue();
            bullet.Initialize(bulletData, context, this);
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        var newBullet = Object.Instantiate(prefab, parent).GetComponent<BulletBase>();
        newBullet.Initialize(bulletData, context, this);
        allBullets.Add(newBullet);
        return newBullet;
    }

    public void Return(BulletBase bullet)
    {
        bullet.gameObject.SetActive(false);
        freeBullets.Enqueue(bullet);

        bullet.transform.parent = parent.transform;
        bullet.transform.position = parent.transform.position;
        OnBulletReturn?.Invoke(bullet);
    }

    public void Clear()
    {
        foreach (var b in allBullets)
        {
            if (b != null)
                b.HandleProjectileDestroy();
        }
        allBullets.Clear();
        freeBullets.Clear();
    }
}
