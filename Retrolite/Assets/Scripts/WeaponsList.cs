using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsList : MonoBehaviour
{
    public Image weaponImage, secondWeaponImage;
    public Image bulletImage, secondBulletImage;
    public GameObject firstWeapon, secondWeapon;
    public Sprite[] bullets;
    public Text[] firstCharacteristics, secondCharacteristics;
    public Gun firstGun, secondGun;
    public SpriteRenderer Reflection1, Reflection2;
    public bool reload = false;
    public GameObject Сursor;

    [SerializeField] private Material _material;
    [SerializeField] private GameObject _reloadBar;
    
    private SpriteRenderer a;
    public AudioClip start,end;
    private AudioSource sound;
    private SpriteRenderer m_SpriteRenderer;

    private void Start()
    {
        a = Сursor.GetComponent<SpriteRenderer>();
        m_SpriteRenderer = firstWeapon.GetComponent<SpriteRenderer>();
        weaponImage.sprite = m_SpriteRenderer.sprite;
        if (Reflection1 != null) Reflection1.sprite = m_SpriteRenderer.sprite;
        m_SpriteRenderer = secondWeapon.GetComponent<SpriteRenderer>();
        secondWeaponImage.sprite = m_SpriteRenderer.sprite;
        if (Reflection2 != null) Reflection2.sprite = m_SpriteRenderer.sprite;
        firstGun = firstWeapon.GetComponent<Gun>();
        secondGun = secondWeapon.GetComponent<Gun>();
        bulletImage.color = new Color(0.2f + Game.Calculate(firstGun.BulletSpeed, firstGun.curentBullet) / 15f, 0.2f + (1 - (Convert.ToSingle(firstGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(firstGun.Damage.Length) / 5), 1);
        bulletImage.sprite = bullets[firstGun.weaponStyle];
        secondBulletImage.color = new Color(0.2f + Game.Calculate(secondGun.BulletSpeed, secondGun.curentBullet) / 15f, 0.2f + (1 - (Convert.ToSingle(secondGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(secondGun.Damage.Length) / 5), 1);
        secondBulletImage.sprite = bullets[secondGun.weaponStyle];
        Formula(firstGun, firstCharacteristics);
        Formula(secondGun, secondCharacteristics);
        sound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (Input.GetAxis("Fire1") >= 0.1f)
        {
            if(firstGun.weaponStyle != 5)
            {
                if (firstGun.ammo != 0 && !reload) {
                    firstGun.Shoot();
                    if(firstGun.weaponStyle != 3 && firstGun.weaponStyle != 5)firstCharacteristics[4].text = Convert.ToString(firstGun.ammo) + "/" + Convert.ToString(firstGun.maxAmmo);
                    else firstCharacteristics[4].text = " ";
                } 
                else StartCoroutine(ReloadGun(firstGun));
            }
        }
        else if (Input.GetAxis("Fire2") >= 0.1f)
        {
            if(secondGun.weaponStyle != 5)
            {
                if (secondGun.ammo != 0 && !reload) {
                    secondGun.Shoot();
                    if(secondGun.weaponStyle != 3 && secondGun.weaponStyle != 5)secondCharacteristics[4].text = Convert.ToString(secondGun.ammo) + "/" + Convert.ToString(secondGun.maxAmmo);
                    else secondCharacteristics[4].text = " ";
                } 
                else StartCoroutine(ReloadGun(secondGun));
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (firstGun.ammo != firstGun.maxAmmo && !reload) StartCoroutine(ReloadGun(firstGun));
            else if (secondGun.ammo != secondGun.maxAmmo && !reload) StartCoroutine(ReloadGun(secondGun));
        }
        Vector3 joy = Vector3.zero;
        
        joy.x = Input.GetAxis("JoyX");
        joy.y = Input.GetAxis("JoyY");

        Сursor.transform.position = transform.position + (joy * 40  );
        a.color = new Color(1,1,1,3 * (Mathf.Abs(joy.x) + Mathf.Abs(joy.y)));
    }
    IEnumerator ReloadGun(Gun gun)
    {
        if(!reload)
        {
            sound.PlayOneShot(start);
            float t = 0;
            reload = true;
            gun.animator.SetBool("Reload", true);
            _reloadBar.SetActive(true);
            while (t < 1)
            {
                t += Time.deltaTime / gun.ReloadTime;
                _material.SetFloat("_Arc2", (1 - t) * 360);
                yield return null;
            }
            _reloadBar.SetActive(false);
            gun.animator.SetBool("Reload", false);
            gun.ammo = gun.maxAmmo;
            firstCharacteristics[4].text = Convert.ToString(firstGun.ammo) + "/" + Convert.ToString(firstGun.maxAmmo);
            secondCharacteristics[4].text = Convert.ToString(secondGun.ammo) + "/" + Convert.ToString(secondGun.maxAmmo);
            sound.PlayOneShot(end);
            reload = false;
        } 
    }
    public void PickUp(Collider2D collision, bool isLeft)
    {
        if(!reload)
        {
            if (isLeft)
            {
                if(firstGun.weaponStyle != 5)firstWeapon.GetComponent<Collider2D>().enabled = true;
                collision.gameObject.transform.parent = firstWeapon.transform.parent;
                collision.gameObject.transform.rotation = firstWeapon.transform.rotation;
                collision.gameObject.transform.position = firstWeapon.transform.position;
                collision.gameObject.transform.localScale = firstWeapon.transform.localScale;
                collision.enabled = false;
                firstGun.enabled = false;
                firstWeapon.tag = "Weapon";  
                firstWeapon.transform.parent = null;
                firstWeapon.transform.localScale = new Vector2(1,1);
                firstWeapon = collision.gameObject;
                firstWeapon.tag = "Empty";  
                firstGun = firstWeapon.GetComponent<Gun>();
                firstGun.enabled = true;
                if(firstGun.weaponStyle != 5){  
                    bulletImage.sprite = bullets[firstGun.weaponStyle];
                    bulletImage.color = new Color(0.2f + Game.Calculate(firstGun.BulletSpeed, firstGun.curentBullet) / 15f, 0.2f + (1 - (Convert.ToSingle(secondGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(firstGun.Damage.Length) / 5), 1);
                }
                else bulletImage.color = new Color(1,1,1,0);
                m_SpriteRenderer = firstWeapon.GetComponent<SpriteRenderer>();
                weaponImage.sprite =  m_SpriteRenderer.sprite;
                if(Reflection1 != null)Reflection1.sprite =  m_SpriteRenderer.sprite;
                Formula(firstGun, firstCharacteristics);
            }
            else {
                if(secondGun.weaponStyle != 5)secondWeapon.GetComponent<Collider2D>().enabled = true;
                collision.gameObject.transform.rotation = secondWeapon.transform.rotation;
                collision.gameObject.transform.position = secondWeapon.transform.position;
                collision.gameObject.transform.parent = secondWeapon.transform.parent;
                collision.gameObject.transform.localScale = secondWeapon.transform.localScale;
                collision.enabled = false;
                secondGun.enabled = false;
                secondWeapon.tag = "Weapon"; 
                secondWeapon.transform.parent = null;
                secondWeapon.transform.localScale = new Vector2(1,1);
                secondWeapon = collision.gameObject;  
                secondWeapon.tag = "Empty";
                secondGun = secondWeapon.GetComponent<Gun>(); 
                secondGun.enabled = true;
                if(secondGun.weaponStyle != 5){  
                    secondBulletImage.sprite = bullets[secondGun.weaponStyle];
                    secondBulletImage.color = new Color(0.2f + Game.Calculate(secondGun.BulletSpeed, secondGun.curentBullet) / 15f, 0.2f + (1 - (Convert.ToSingle(secondGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(secondGun.Damage.Length) / 5), 1);
                }
                else secondBulletImage.color = new Color(1,1,1,0);
                m_SpriteRenderer = secondWeapon.GetComponent<SpriteRenderer>();
                secondWeaponImage.sprite =  m_SpriteRenderer.sprite;
                if(Reflection2 != null)Reflection2.sprite =  m_SpriteRenderer.sprite;
                Formula(secondGun, secondCharacteristics);
            }
        }
    }
    private void Formula(Gun gunInfo,Text[] characteristics)
    {
        characteristics[0].text = gunInfo.ShootSpeed;
        characteristics[1].text = gunInfo.BulletSpeed;
        if(gunInfo.weaponStyle != 3 && gunInfo.weaponStyle != 5)characteristics[4].text = Convert.ToString(gunInfo.ammo) + "/" + Convert.ToString(gunInfo.maxAmmo);
        else characteristics[4].text = " ";
        switch (gunInfo.weaponStyle){
                case 0:
                    characteristics[2].text = "Plasma";
                    break;
                case 1:
                    characteristics[2].text = "Bombs";
                    break;
                case 2:
                    characteristics[2].text = "Sound wave";
                    break;
                case 3:
                    characteristics[2].text = "Laser";
                    break; 
                case 4:
                    characteristics[2].text = "Tesla";
                    break; 
                case 5:
                    characteristics[2].text = "None";
                    break; 
            }
        characteristics[3].text = gunInfo.Damage;
    }
}
