using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public static Run Current = null;
    public static SavingSystem instance;

    private static float StartTime;

    private void Start()
    {
        instance = this;
        StartTime = Time.time;
        if(Current == null) Current = new Run(Game.PlayerHealth.healthPoint, Game.Sanity.Sanity, Game.List.firstGun, Game.List.secondGun);
        else LoadRun();
    }
    private void LoadRun()
    {
        Game.PlayerHealth.lifes = Run.Lives; 
        Game.PlayerHealth.healthPoint = Run.Health; 
        Game.PlayerHealth.maxHealthPoint = Run.MaxHealth;
        Game.Sanity.Sanity = Run.Sanity;
        Game.Money.money = Run.Money;

        Game.List.firstGun.Damage = Run.FirstWeapon.Damage;
        Game.List.firstGun.weaponStyle = Run.FirstWeapon.weaponStyle;
        Game.List.firstGun.ShootSpeed = Run.FirstWeapon.ShootSpeed;
        Game.List.firstGun.BulletSpeed = Run.FirstWeapon.BulletSpeed;
        Game.List.firstGun.kills = Run.FirstWeapon.kills;
        Game.List.firstGun.ammo = Run.FirstWeapon.ammo;
        Game.List.firstGun.maxAmmo = Run.FirstWeapon.maxAmmo;
        Game.List.firstGun.PureDamage = Run.FirstWeapon.PureDamage;

        Game.List.secondGun.Damage = Run.SecondWeapon.Damage;
        Game.List.secondGun.weaponStyle = Run.SecondWeapon.weaponStyle;
        Game.List.secondGun.ShootSpeed = Run.SecondWeapon.ShootSpeed;
        Game.List.secondGun.BulletSpeed = Run.SecondWeapon.BulletSpeed;
        Game.List.secondGun.kills = Run.SecondWeapon.kills;
        Game.List.secondGun.ammo = Run.SecondWeapon.ammo;
        Game.List.secondGun.maxAmmo = Run.SecondWeapon.maxAmmo;
        Game.List.secondGun.PureDamage = Run.SecondWeapon.PureDamage;

        Game.List.firstGun.Awake();
        Game.List.secondGun.Awake();
    }
    public static void SaveRun()
    {
        Run.Stage++;
        Run.Lives = Game.PlayerHealth.lifes; 
        Run.Money = Game.Money.money;
        Run.Health = Game.PlayerHealth.healthPoint; 
        Run.MaxHealth = Game.PlayerHealth.maxHealthPoint; 
        Run.Sanity = Game.Sanity.Sanity;
        Run.Time =+ Time.time - StartTime;
        Run.FirstWeapon = Game.List.firstGun;
        Run.SecondWeapon = Game.List.secondGun;
    }
    
}

public class Run
{
    public static int Stage;
    public static int Lives;
    public static int Money;
    public static float Time;
    public static float Health, MaxHealth;
    public static float Sanity;
    public static Gun FirstWeapon, SecondWeapon;

    public Run(float health, float sanity, Gun firstGun, Gun secondGun)
    {
        Stage = 0;
        Lives = 0;
        Time = 0;
        Money = 0;
        Health = health;
        MaxHealth = health;
        Sanity = sanity;
        FirstWeapon = firstGun;
        SecondWeapon = secondGun;
    }
}