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
        firstGun.randomNumber = DataHolder.FirstWeapon.randomNumber;
        firstGun.randomOperand = DataHolder.FirstWeapon.randomOperand;
        firstGun.MagicNumbers = DataHolder.FirstWeapon.MagicNumbers;
        firstGun.numOfOperand = DataHolder.FirstWeapon.numOfOperand;
        firstGun.weaponStyle = DataHolder.FirstWeapon.weaponStyle;
        firstGun.shootSpeed = DataHolder.FirstWeapon.shootSpeed;
        firstGun.bulletSpeed = DataHolder.FirstWeapon.bulletSpeed;
        firstGun.kills = DataHolder.FirstWeapon.kills;
        firstGun.ammo = DataHolder.FirstWeapon.ammo;
        firstGun.maxAmmo = DataHolder.FirstWeapon.maxAmmo;
        secondGun.randomNumber = DataHolder.SecondWeapon.randomNumber;
        secondGun.randomOperand = DataHolder.SecondWeapon.randomOperand;
        secondGun.MagicNumbers = DataHolder.SecondWeapon.MagicNumbers;
        secondGun.numOfOperand = DataHolder.SecondWeapon.numOfOperand;
        secondGun.weaponStyle = DataHolder.SecondWeapon.weaponStyle;
        secondGun.shootSpeed = DataHolder.SecondWeapon.shootSpeed;
        secondGun.bulletSpeed = DataHolder.SecondWeapon.bulletSpeed;
        secondGun.kills = DataHolder.SecondWeapon.kills;
        secondGun.ammo = DataHolder.SecondWeapon.ammo;
        secondGun.maxAmmo = DataHolder.SecondWeapon.maxAmmo;
        firstGun.Awake();
        secondGun.Awake();
    }
}
