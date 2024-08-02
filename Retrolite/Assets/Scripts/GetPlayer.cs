using UnityEngine;

public class GetPlayer : MonoBehaviour
{
    private Money coins;
    private Health health;
    private SanitySystem sanity;
    private Gun firstGun,secondGun;
    public GameObject firstWeapon, secondWeapon;
    private void Awake()
    {
        Vector2 pos = new Vector2(0,0);
        transform.position = pos;
        coins = GetComponent<Money>();
        health = GetComponent<Health>();
        sanity = GetComponent<SanitySystem>();
        firstGun = firstWeapon.GetComponent<Gun>();
        secondGun = secondWeapon.GetComponent<Gun>();
        sanity.Sanity = DataHolder.Sanity;
        coins.money = DataHolder.money;
        health.healthPoint = DataHolder.Health;
        health.maxHealthPoint = DataHolder.MaxHealth;
        health.lifes = DataHolder.lifes;
        firstGun.Damage = DataHolder.FirstWeapon.Damage;
        firstGun.weaponStyle = DataHolder.FirstWeapon.weaponStyle;
        firstGun.ShootSpeed = DataHolder.FirstWeapon.ShootSpeed;
        firstGun.BulletSpeed = DataHolder.FirstWeapon.BulletSpeed;
        firstGun.kills = DataHolder.FirstWeapon.kills;
        firstGun.ammo = DataHolder.FirstWeapon.ammo;
        firstGun.maxAmmo = DataHolder.FirstWeapon.maxAmmo;
        firstGun.PureDamage = DataHolder.FirstWeapon.PureDamage;
        secondGun.Damage = DataHolder.SecondWeapon.Damage;
        secondGun.weaponStyle = DataHolder.SecondWeapon.weaponStyle;
        secondGun.ShootSpeed = DataHolder.SecondWeapon.ShootSpeed;
        secondGun.BulletSpeed = DataHolder.SecondWeapon.BulletSpeed;
        secondGun.kills = DataHolder.SecondWeapon.kills;
        secondGun.ammo = DataHolder.SecondWeapon.ammo;
        secondGun.maxAmmo = DataHolder.SecondWeapon.maxAmmo;
        secondGun.PureDamage = DataHolder.SecondWeapon.PureDamage;
        firstGun.Awake();
        secondGun.Awake();
    }
}
