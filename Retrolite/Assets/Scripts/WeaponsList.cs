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
    public AudioClip start,end;
    private AudioSource sound;
    private SpriteRenderer m_SpriteRenderer;

    private void Start()
    {
        m_SpriteRenderer = firstWeapon.GetComponent<SpriteRenderer>();
        weaponImage.sprite = m_SpriteRenderer.sprite;
        if (Reflection1 != null) Reflection1.sprite = m_SpriteRenderer.sprite;
        m_SpriteRenderer = secondWeapon.GetComponent<SpriteRenderer>();
        secondWeaponImage.sprite = m_SpriteRenderer.sprite;
        if (Reflection2 != null) Reflection2.sprite = m_SpriteRenderer.sprite;
        firstGun = firstWeapon.GetComponent<Gun>();
        secondGun = secondWeapon.GetComponent<Gun>();
        bulletImage.color = new Color(0.2f + (firstGun.bulletSpeed / 15), 0.2f + (1 - (Convert.ToSingle(firstGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(firstGun.numOfOperand) / 5), 1);
        bulletImage.sprite = bullets[firstGun.weaponStyle];
        secondBulletImage.sprite = bullets[secondGun.weaponStyle];
        Formula(firstGun, firstCharacteristics);
        Formula(secondGun, secondCharacteristics);
        sound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (firstGun.ammo != 0 && !reload) {
                firstGun.Shoot();
                if(firstGun.weaponStyle != 3 && firstGun.weaponStyle != 5)firstCharacteristics[4].text = Convert.ToString(firstGun.ammo) + "/" + Convert.ToString(firstGun.maxAmmo);
                else firstCharacteristics[4].text = " ";
            } 
            else StartCoroutine(ReloadGun(firstGun));
        }
        else if (Input.GetMouseButton(1))
        {
            if (secondGun.ammo != 0 && !reload) {
                secondGun.Shoot();
                if(secondGun.weaponStyle != 3 && secondGun.weaponStyle != 5)secondCharacteristics[4].text = Convert.ToString(secondGun.ammo) + "/" + Convert.ToString(secondGun.maxAmmo);
                else secondCharacteristics[4].text = " ";
            } 
            else StartCoroutine(ReloadGun(secondGun));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (firstGun.ammo != firstGun.maxAmmo && !reload) StartCoroutine(ReloadGun(firstGun));
            else if (secondGun.ammo != secondGun.maxAmmo && !reload) StartCoroutine(ReloadGun(secondGun));
        }
    }
    IEnumerator ReloadGun(Gun gun)
    {
        if(!reload)
        {
            sound.PlayOneShot(start);
            float t = Time.time;
            reload = true;
            gun.animator.SetBool("Reload", true);
            while (Time.time - t < gun.reloadTime)
            {
                yield return null;
            }
            gun.animator.SetBool("Reload", false);
            gun.ammo = gun.maxAmmo;
            firstCharacteristics[4].text = Convert.ToString(firstGun.ammo) + "/" + Convert.ToString(firstGun.maxAmmo);
            secondCharacteristics[4].text = Convert.ToString(secondGun.ammo) + "/" + Convert.ToString(secondGun.maxAmmo);
            sound.PlayOneShot(end);
            reload = false;
        } 
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Weapon") 
        {
            if (Input.GetKeyDown(KeyCode.E) && !reload)
            {
                firstWeapon.GetComponent<Collider2D>().enabled = true;
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
                firstGun.weaponClip = "Clip0";
                if(firstGun.weaponStyle != 5){  
                    bulletImage.sprite = bullets[firstGun.weaponStyle];
                    bulletImage.color = new Color(0.2f + (firstGun.bulletSpeed / 15), 0.2f + (1 - (Convert.ToSingle(firstGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(firstGun.numOfOperand) / 5), 1);
                }
                else bulletImage.color = new Color(1,1,1,0);
                m_SpriteRenderer = firstWeapon.GetComponent<SpriteRenderer>();
                weaponImage.sprite =  m_SpriteRenderer.sprite;
                if(Reflection1 != null)Reflection1.sprite =  m_SpriteRenderer.sprite;
                Formula(firstGun, firstCharacteristics);
            }
            else if (Input.GetKeyDown(KeyCode.Q) && !reload)
            {
                collision.gameObject.transform.rotation = secondWeapon.transform.rotation;
                collision.gameObject.transform.position = secondWeapon.transform.position;
                collision.gameObject.transform.localScale = secondWeapon.transform.localScale;
                collision.gameObject.transform.parent = secondWeapon.transform.parent;
                secondGun.enabled = false;
                secondWeapon.tag = "Weapon"; 
                secondWeapon.transform.parent = null;
                secondWeapon = collision.gameObject;  
                secondWeapon.tag = "Empty";
                secondGun = secondWeapon.GetComponent<Gun>(); 
                secondGun.enabled = true;
                secondGun.weaponClip = "Clip1";
                if(secondGun.weaponStyle != 5){  
                    secondBulletImage.sprite = bullets[secondGun.weaponStyle];
                    secondBulletImage.color = new Color(0.2f + (secondGun.bulletSpeed / 15), 0.2f + (1 - (Convert.ToSingle(secondGun.weaponStyle) / 5)), 0.2f + (Convert.ToSingle(secondGun.numOfOperand) / 5), 1);
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
        characteristics[0].text = Convert.ToString(gunInfo.shootSpeed);
        characteristics[1].text = Convert.ToString(gunInfo.bulletSpeed);
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
        string form = null;
        for (int i = 0;i < gunInfo.numOfOperand; i++){
            switch(gunInfo.randomNumber[i]){
                case 0:
                    form += "R";
                    break;
                case 1:
                    form += "H";
                    break;
                case 2:
                    form += "D";
                    break;
                case 3:
                    form += "T";
                    break;
                case 4:
                    form += "M";
                    break;
                case 5:
                    form += "P";
                    break;
                case 6:
                    form += "E";
                    break;
                case 7:
                    form += "A";
                    break;
                case 8:
                    form += "K";
                    break;
                case >= 9:
                    if(i != 0){
                        if(gunInfo.MagicNumbers[i] < 0) {
                            form +=  "(" + Convert.ToString(gunInfo.MagicNumbers[i]) + ")";
                            break;
                        }
                    }
                    form += Convert.ToString(gunInfo.MagicNumbers[i]);
                    break;
            }
            if (i != gunInfo.numOfOperand - 1)
            switch(gunInfo.randomOperand[i]){
                case 0:
                    form += "+";
                    break;
                case 1:
                    form += "-";
                    break;
                case 2:
                    form += "*";
                    break;
                case 3:
                    form += "/";
                    break;
            }
        }
        characteristics[3].text = form;
    }
}
