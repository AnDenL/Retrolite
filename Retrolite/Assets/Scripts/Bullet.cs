using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet stats")]

    public Sprite[] bulletImage;
    public int bulletStyle, Number;
    public GameObject Effect, effects;
    public float BulletSpeed;
    public Vector2 startPos;
    public float Echo;

    [Header("Reference")]

    public Health Health;
    public Gun Gun;

    [SerializeField] private  LayerMask enemyLayers;
    [SerializeField] private GameObject bulletTrail, explosionEffect, elctroEffect;

    private GameObject player;
    private Money money;
    private Health health;
    private float timeAfterShoot;
    private Rigidbody2D Rigidbody;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>(); 
        Rigidbody.simulated = false;
    }

    public void Shoot(float speed, float range, int number, Gun gun)
    {
        BulletSpeed = Mathf.Clamp(speed, 0.1f, 15f);
        range = Mathf.Clamp(range, 1, 25);
        Number = number;
        this.Gun = gun;
        bulletStyle = gun.weaponStyle;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = bulletImage[bulletStyle];
        sprite.color = new Color(0.2f + (BulletSpeed/15), 0.2f + (1 - (Convert.ToSingle(bulletStyle) / 5)),0.2f + (Convert.ToSingle(Gun.Damage.Length) / 5),1);
        transform.parent = null;
        timeAfterShoot = Time.time;
        startPos = transform.position;
        Rigidbody.simulated = true;
        Rigidbody.AddForce(transform.right * speed, ForceMode2D.Impulse);
        Invoke("BulletDisappear",(range / 5) / BulletSpeed);
        sprite.enabled = true;
        transform.localScale = new Vector2(1,1);;
        if(bulletStyle == 3){
            bulletTrail.SetActive(true);
        
            TrailRenderer trailRenderer = bulletTrail.GetComponent<TrailRenderer>();

            trailRenderer.startColor = GetComponent<SpriteRenderer>().color;
            trailRenderer.endColor = GetComponent<SpriteRenderer>().color;
        }
        else if(bulletStyle == 2)StartCoroutine(Scale());
        else if(bulletStyle == 4){
            elctroEffect.SetActive(true);
            ParticleSystem particle = elctroEffect.GetComponent<ParticleSystem>();

            var mainModule = particle.main;
            mainModule.startColor = GetComponent<SpriteRenderer>().color;
        }
        Echo = gun.Echo;
        gun.Echo = 0;
    }
    IEnumerator Scale()
    {
        while(transform.localScale.x < 3)
        {
            float scale = 0.5f + Time.time - timeAfterShoot;
            transform.localScale = new Vector2(scale,scale);
            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {  
        if(collision.tag != "Player" && collision.tag != "Weapon" && collision.tag != "Empty")
        {
            if(collision.tag == "Enemy")
            {
                float damage = Game.Calculate(Gun.Damage, this);
                Health = collision.gameObject.GetComponent<Health>();
                if(bulletStyle < 2)
                {
                    Hit(Health, damage);
                    BulletDisappear();
                }
                else if (bulletStyle == 4) 
                {
                    Hit(Health, damage);
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 5f, enemyLayers);
                    int i = 0;
                        while (hitEnemies[i].GetComponent<Health>().isInvincible) {
                            if(hitEnemies.Length > i + 1)i++;
                            else break;
                        }
                    if (i < hitEnemies.Length - 1)
                    {
                        Rigidbody.velocity = new Vector2(0, 0);
                        Vector2 direction = hitEnemies[i].transform.position - transform.position;
                        float rotateZ = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);
                        Rigidbody.AddForce(transform.right * BulletSpeed, ForceMode2D.Impulse);
                    }
                    else Destroy(gameObject,0.2f);
                }
                else Hit(Health, damage);
            }
            else if (collision.tag == "Tilemap")
            {
                BulletDisappear();
            }
        }
    }

    private void Hit(Health hp, float damage)
    {
        if(hp.SetHealth(damage,Gun.PureDamage) == true) Gun.kills++;
    }

    private void BulletDisappear()
    {
        if (transform.childCount > 0) {
            GameObject effect = Instantiate(Effect, transform.GetChild(0));
            ParticleSystem particle = effect.GetComponent<ParticleSystem>();

            var mainModule = particle.main;
            mainModule.startColor = GetComponent<SpriteRenderer>().color;

            effects.transform.parent = null;

            Destroy(effects, 1);

            if (bulletStyle == 1) 
            {
                explosionEffect.SetActive(true);
                Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(transform.position, 1.2f, enemyLayers);
                float damage = Game.Calculate(Gun.Damage, this);
                foreach(Collider2D enemy in hitEmenies)
                {
                    Hit(enemy.GetComponent<Health>(), damage);
                    Gun.Echo += damage;
                } 
            }

            Destroy(gameObject);
        } 
    }
}
