using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float Echo, PureDamage, Spread, ReloadTime;
    public Sprite[] Image;
    public bool randomizeWeapon = true;
    public int weaponStyle, maxAmmo, ammo, kills;
    public string ShootSpeed, BulletSpeed, Range, Damage;
    [Header("Reference")]
    public GameObject bulletPrefab;
    public AudioClip[] shootSound;
    public Animator animator;
    public Bullet curentBullet;

    private SpriteRenderer Sprite;
    private PlayerMove player;
    private AudioSource sound;
    private float shootTime;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>(); ;
        Sprite = GetComponent<SpriteRenderer>();
        if(randomizeWeapon){
            RandomParameter();
        }   
        Sprite.sprite = Image[weaponStyle];
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        if (weaponStyle != 5)CreateBullet();
        maxAmmo = ammo;
    }
    public void Shoot()
    {
        if (Time.time >= shootTime)
        {
            if (weaponStyle != 5 && Time.timeScale != 0f)
            {
                sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                sound.PlayOneShot(shootSound[weaponStyle]);
                float r = UnityEngine.Random.Range(-Spread, Spread);
                if (player.horizontalMove != 0 || player.verticalMove != 0) r += r * (player.speed / 2);

                float bulletSpeed = Mathf.Abs(Game.Calculate(BulletSpeed, curentBullet));
                float range = Mathf.Abs(Game.Calculate(Range, curentBullet));

                curentBullet.transform.localRotation = Quaternion.Euler(0, 0, r);
                curentBullet.Shoot(bulletSpeed, range, ammo, this);
                animator.SetTrigger("Shoot");
                shootTime = Time.time + 1f / Mathf.Clamp(Mathf.Abs(Game.Calculate(ShootSpeed, curentBullet)), 0.3f, 20f);
                if (weaponStyle != 3)
                {
                    ammo--;
                }
                CreateBullet();
            }
        }
    }
    private void CreateBullet()
    {
        curentBullet = Instantiate(bulletPrefab, transform.GetChild(0)).GetComponent<Bullet>();
    }
    private void RandomParameter()
    {
        weaponStyle = Random.Range(0, 5);
        ReloadTime = Random.Range(0.5f, 5f);
        Spread = Random.Range(1, 12);
        Range = RandomExpression(9, 15);
        BulletSpeed = RandomExpression(0, 15);
        ShootSpeed = RandomExpression(0, 15);
        Damage = RandomExpression(0, 21);
        int ammoAmount = Random.Range(1, 31);
        maxAmmo = ammoAmount;
        ammo = ammoAmount;

    }

    private void SetDamage(string value)
    {
        Damage = value;
    }

    private void SetRange(string value)
    {
        Range = value;
    }

    private void SetBulletSpeed(string value)
    {
        BulletSpeed = value;
    }

    private void SetSpread(float value)
    {
        Spread = value;
    }

    private void SetShootSpeed(string value)
    {
        ShootSpeed = value;
    }

    private void SetReloadTime(float value)
    {
        ReloadTime = value;
    }

    private void SetMaxAmmo(int value)
    {
        maxAmmo = value;
    }

    private string RandomExpression(int min, int max)
    {
        int length = 1;
        if(Random.Range(1, 3) == 1) length = Random.Range(1, 4);

        string expression = "";
        for (int i = 0; i < length; i++)
        {
            int r = Random.Range(min,max);

            switch(r)
            {
                case 0:
                    expression += "-5";
                    break;
                case 1:
                    expression += "-4";
                    break;
                case 2:
                    expression += "-3";
                    break;
                case 3:
                    expression += "-2";
                    break;
                case 4:
                    expression += "-1";
                    break;
                case 5:
                    expression += "0";
                    break;
                case 6:
                    expression += "0,5";
                    break;
                case 7:
                    expression += "1";
                    break;
                case 8:
                    expression += "2";
                    break;
                case 9:
                    expression += "3";
                    break;
                case 10:
                    expression += "4";
                    break;
                case 11:
                    expression += "5";
                    break;
                case 12:
                    expression += "R";
                    break;
                case 13:
                    expression += "P";
                    break;
                case 17:
                    expression += "S";
                    break;
                case 14:
                    expression += "E";
                    break;
                case 15:
                    expression += "N";
                    break;
                case 16:
                    expression += "M";
                    break;
                case 18:
                    expression += "K";
                    break;
                case 19:
                    expression += "D";
                    break;
                case 20:
                    expression += "H";
                    break;
                
            }
            expression += " ";
            if(i < length - 1)
            {
                int o = Random.Range(0,4);

                switch(o)
                {
                    case 0:
                        expression += "+";
                        break;
                    case 1:
                        expression += "-";
                        break;
                    case 2:
                        expression += "*";
                        break;
                    case 3:
                        expression += "/";
                        break;
                }
                expression += " ";
            }
        }
        return expression;
    }
}