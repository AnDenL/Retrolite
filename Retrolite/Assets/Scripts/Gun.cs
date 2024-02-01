using System;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public int[] randomNumber = new int[]{0,0,0,0};
    public int[] randomOperand = new int[]{0,0,0,0};
    public int[] MagicNumbers = new int[]{0,0,0,0};
    public Sprite[] Image;
    public bool randomizeWeapon = true;
    public int numOfOperand, weaponStyle, maxAmmo, ammo, kills;
    public float shootSpeed, bulletSpeed, reloadTime, spread, range, echo = 0;
    [Header("Reference")]
    public GameObject bulletPrefab;
    public AudioClip[] shootSound;
    public Animator animator;
    public string weaponClip;

    private SpriteRenderer Sprite;
    private PlayerMove player;
    private AudioSource sound;
    private float shootTime;
    private Transform clip;
    private Bullet curentBullet;
    private Gun thisGun;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>(); ;
        thisGun = GetComponent<Gun>();
        Sprite = GetComponent<SpriteRenderer>();
        if(randomizeWeapon){
            RandomizeWeapon();
        }   
        Sprite.sprite = Image[weaponStyle];
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        GameObject clipo = GameObject.Find(weaponClip);
        clip = clipo.GetComponent<Transform>();
        if (weaponStyle != 5)CreateBullet();
    }
    public void Shoot()
    {
        if (Time.time >= shootTime)
        {
            if (weaponStyle != 5 && Time.timeScale != 0f)
            {
                sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                sound.PlayOneShot(shootSound[weaponStyle]);
                float r = UnityEngine.Random.Range(-spread, spread);
                if (player.horizontalMove != 0 || player.verticalMove != 0) r += r * (player.speed / 2);
                curentBullet.transform.localRotation = Quaternion.Euler(0, 0, r);
                curentBullet.Shoot();
                CreateBullet();
                animator.SetTrigger("Shoot");
                shootTime = Time.time + 1f / shootSpeed;
                if (weaponStyle != 3)
                {
                    ammo--;
                }
            }
        }
    }
    void CreateBullet()
    {
        curentBullet = Instantiate(bulletPrefab, clip.GetChild(0)).GetComponent<Bullet>();
        curentBullet.bulletStyle = weaponStyle;
        curentBullet.bulletSpeed = bulletSpeed;
        curentBullet.numOfOperand = numOfOperand;
        curentBullet.randomNumber = randomNumber;
        curentBullet.randomOperand = randomOperand;
        curentBullet.MagicNumbers = MagicNumbers;
        curentBullet.echo = echo;
        curentBullet.range = range;
        curentBullet.gun = thisGun;
        curentBullet.kills = kills;
        if(ammo == 0) curentBullet.ammo = maxAmmo;
        else curentBullet.ammo = ammo;
    }
    private void RandomizeWeapon()
    {
        spread = UnityEngine.Random.Range(0f, 4f); ;
        weaponStyle = UnityEngine.Random.Range(0,5);
        numOfOperand = UnityEngine.Random.Range(1,5);
        range = UnityEngine.Random.Range(4,15);
        for(int i = 0;i < numOfOperand - 1; i++)
        {
            randomNumber[i + 1] = UnityEngine.Random.Range(0,16);
            if (randomNumber[i + 1] == 7 && weaponStyle == 3) randomNumber[i + 1] = 1;
            randomOperand[i] = UnityEngine.Random.Range(0,4);
            if (randomNumber[i + 1] >= 8) MagicNumbers[i + 1] = UnityEngine.Random.Range(-5,6);
        }
        randomNumber[0] = UnityEngine.Random.Range(0,16);
        if (randomNumber[0] >= 8) MagicNumbers[0] = UnityEngine.Random.Range(-5,6);
        bulletSpeed = UnityEngine.Random.Range(3f,8f);
        shootSpeed = UnityEngine.Random.Range(2f,6f);
        ammo = UnityEngine.Random.Range(4, 18);
        maxAmmo = ammo;
        reloadTime = UnityEngine.Random.Range(0.2f, 0.8f) * Mathf.Sqrt(ammo);
        switch(weaponStyle){
            case 1:
                shootSpeed /= 2;
                break;
            case 2:
                bulletSpeed /= 2;
                break;
            case 3:
                bulletSpeed = 15;
                break;
            case 4:
                bulletSpeed = UnityEngine.Random.Range(12f,14f);
                break;
            case 5:
                Sprite.color = new Color(1,1,1,0);
                break;
        }
    }
}
