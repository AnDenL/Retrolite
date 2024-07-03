using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Transform _firePosition;
    [SerializeField] protected WeaponItem _weaponData;

    protected Bullet[] _bullets;
    protected float _fireTime; 
    protected bool _canShoot = true;

    public int BulletsLast { private set; get;}
    public float EchoDamage;

    private void Start()
    {
        Game.Weapon = this;
        _bullets = new Bullet[0];
    }

    public void NewWeapon(WeaponItem item)
    {
        _weaponData = item;
        foreach(Bullet obj in _bullets)
        {
            Destroy(obj.gameObject);
        }
        _bullets = new Bullet[_weaponData.Magazine];    
        GetComponent<SpriteRenderer>().sprite = _weaponData.Icon;
        BulletsLast = _weaponData.Magazine - 1;
        CreateMagazine();
    }

    public virtual void Fire()
    {
        if(_canShoot && _fireTime < Time.time)
        {
            float r = Random.Range(-_weaponData.Spread, _weaponData.Spread);

            _bullets[BulletsLast].Return(this.transform);
            _bullets[BulletsLast].transform.localRotation = Quaternion.Euler(0, 0, r);
            _bullets[BulletsLast].Shoot(Mathf.Abs(Game.Calculate(_weaponData.BulletSpeed, _bullets[BulletsLast])),Mathf.Abs(Game.Calculate(_weaponData.Range, _bullets[BulletsLast])));
            
            _fireTime = Time.time + (1 / Mathf.Abs(Game.Calculate(_weaponData.FireSpeed, _bullets[BulletsLast])));
            if(_fireTime > Time.time + 10) _fireTime = Time.time + 10;
            BulletsLast--;
            if(BulletsLast < 0) StartCoroutine(Reload());
        }
    }

    public void ReloadWeapon()
    {
        if(_canShoot && BulletsLast != _weaponData.Magazine - 1) StartCoroutine(Reload());
    }

    protected virtual IEnumerator Reload()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_weaponData.ReloadTime);
        BulletsLast = _weaponData.Magazine - 1;
        _canShoot = true;
    }

    protected virtual void CreateMagazine()
    {
        for(int i = 0; _weaponData.Magazine > i; i++)
        {
            _bullets[i] = Instantiate(_weaponData.BulletPrefab, _firePosition).GetComponent<Bullet>();
            _bullets[i].CreateBullet(i + 1, _weaponData.Damage, this);
        }
    }
}
