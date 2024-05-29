using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet stats")]

    public Sprite[] bulletImage;
    public float bulletSpeed, echo, ammo, kills, range, pureDamage = 0;
    public int bulletStyle, numOfOperand;
    public int[] MagicNumbers = new int[]{0,0,0,0};
    public int[] randomNumber = new int[]{0,0,0,0};
    public int[] randomOperand = new int[]{0,0,0,0};
    public GameObject Effect, effects;

    [Header("Reference")]

    public LayerMask enemyLayers;
    public GameObject bulletTrail, explosionEffect, elctroEffect;
    public Gun gun;
    private GameObject player;
    private Money money;
    private Health health;
    private float timeAfterShoot;
    private Vector2 startPos;
    private Rigidbody2D Rigidbody;
    private SpriteRenderer sprite;

    void Start()
    {
        gun.echo = 0;
        Rigidbody = GetComponent<Rigidbody2D>(); 
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = bulletImage[bulletStyle];
        sprite.color = new Color(0.2f + (bulletSpeed/15), 0.2f + (1 - (Convert.ToSingle(bulletStyle) / 5)),0.2f + (Convert.ToSingle(numOfOperand) / 5),1);
        player = GameObject.FindWithTag("Player");
        money = player.GetComponent<Money>(); 
        health = player.GetComponent<Health>(); 
    }
    public void Shoot()
    {
        transform.parent = null;
        timeAfterShoot = Time.time;
        startPos = transform.position;
        Rigidbody.simulated = true;
        Rigidbody.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
        Invoke("BulletDisappear",(range / 5) / bulletSpeed);
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
                Health hp = collision.gameObject.GetComponent<Health>();
                if(bulletStyle == 1)BulletDisappear();
                else if (bulletStyle == 4) 
                {
                    TakeDamage(hp);
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
                        Rigidbody.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
                    }
                    else Destroy(gameObject,0.2f);
                }
                else TakeDamage(hp);
            }
            else if (collision.tag == "Tilemap")
            {
                Rigidbody.simulated = false;
                if(bulletStyle == 1)AreaTakeDamage();
                else BulletDisappear();
            }
        }
    }
    void TakeDamage(Health EnemyHealth)
    {
        float damage = 0;
        float[] numbers = new float [numOfOperand];
        char[] operations = new char[numOfOperand - 1];

        for (int i = 0;i < numOfOperand; i++)
            switch(randomNumber[i]){
                case 0:
                    numbers[i] = UnityEngine.Random.Range(-5f,6f);
                    break;
                case 1:
                    numbers[i] = EnemyHealth.healthPoint/EnemyHealth.maxHealthPoint;
                    break;
                case 2:
                    numbers[i] = Vector2.Distance(startPos,transform.position);
                    break;
                case 3:
                    numbers[i] = Time.time - timeAfterShoot;
                    break;
                case 4:
                    numbers[i] = money.money/100f;
                    break;
                case 5:
                    numbers[i] = health.healthPoint/health.maxHealthPoint;
                    break;
                case 6:
                    numbers[i] = echo;
                    break;
                case 7:
                    numbers[i] = ammo;
                    break;
                case 8:
                    numbers[i] = kills/10f;
                    break;
                case >= 9:
                    numbers[i] = MagicNumbers[i];
                    break;
            }
        for (int i = 0; i < numOfOperand - 1; i++)
        {
            switch (randomOperand[i])
            {
                case 0:
                    operations[i] = '+';
                    break;
                case 1:
                    operations[i] = '-';
                    break;
                case 2:
                    operations[i] = '*';
                    break;
                case 3:
                    operations[i] = '/';
                    break;
            }
        }
        for (int i = 0; i < numOfOperand - 1; i++) {
            if (i + 1 < numbers.Length) {
                if (operations[i] == '*' || operations[i] == '/') {
                    float tempResult;
                    if (operations[i] == '*') {
                        tempResult = numbers[i] * numbers[i + 1];
                    } else if (numbers[i + 1] == 0) {
                        health.SetHealth(2);
                        return;
                    } else {
                        tempResult = numbers[i] / numbers[i + 1];
                    }
                    numbers[i] = tempResult;
                    Array.Copy(numbers, i + 2, numbers, i + 1, numbers.Length - i - 2);
                    Array.Copy(operations, i + 1, operations, i, operations.Length - i - 1);
                    Array.Resize(ref numbers, numbers.Length - 1);
                    Array.Resize(ref operations, operations.Length - 1);
                    i--;
                }
            }
        }

        damage = numbers[0];
        if (numOfOperand > 1) {
            for (int i = 1; i < numbers.Length; i++) {
                if (operations[i - 1] == '+') {
                    damage += numbers[i];
                } else if (operations[i - 1] == '-') {
                    damage -= numbers[i];
                }
            }
        }
        if(EnemyHealth.SetHealth(damage,pureDamage) == true) gun.kills++;
        gun.echo = damage;
        if(bulletStyle == 3){
            Destroy(gameObject,0.2f); 
            Rigidbody.simulated = false;
        }
        else if (bulletStyle != 2 && bulletStyle != 4) BulletDisappear();
    }
    void AreaTakeDamage()
    {
        float damage = 0;
        float[] numbers = new float [numOfOperand];
        char[] operations = new char[numOfOperand - 1];
        Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(transform.position, 1.2f, enemyLayers);
        explosionEffect.SetActive(true);
        Rigidbody.simulated = false;

        for (int i = 0;i < numOfOperand; i++)
            switch(randomNumber[i]){
                case 0:
                    numbers[i] = UnityEngine.Random.Range(-5f,6f);
                    break;
                case 1:
                    float average = 0;
                    foreach(Collider2D enemy in hitEmenies){
                        Health hp = enemy.GetComponent<Health>(); 
                        average += hp.healthPoint/hp.maxHealthPoint;;
                    } 
                    numbers[i] = average/hitEmenies.Length;
                    break;
                case 2:
                    numbers[i] = Vector2.Distance(startPos,transform.position);
                    break;
                case 3:
                    numbers[i] = Time.time - timeAfterShoot;
                    break;
                case 4:
                    numbers[i] = money.money/10f;
                    break;
                case 5:
                    numbers[i] = health.healthPoint/health.maxHealthPoint;
                    break;
                case 6:
                    numbers[i] = echo;
                    break;
                case 7:
                    numbers[i] = ammo;
                    break;
                case 8:
                    numbers[i] = kills/10f;
                    break;
                case >= 9:
                    numbers[i] = MagicNumbers[i];
                    break;
            }
        for (int i = 0; i < numOfOperand - 1; i++)
        {
            switch (randomOperand[i])
            {
                case 0:
                    operations[i] = '+';
                    break;
                case 1:
                    operations[i] = '-';
                    break;
                case 2:
                    operations[i] = '*';
                    break;
                case 3:
                    operations[i] = '/';
                    break;
            }
        }
        for (int i = 0; i < numOfOperand - 1; i++) {
            if (i + 1 < numbers.Length) {
                if (operations[i] == '*' || operations[i] == '/') {
                    float tempResult;
                    if (operations[i] == '*') {
                        tempResult = numbers[i] * numbers[i + 1];
                    } else if (numbers[i + 1] == 0) {
                        health.SetHealth(2);
                        return;
                    } else {
                        tempResult = numbers[i] / numbers[i + 1];
                    }
                    numbers[i] = tempResult;
                    Array.Copy(numbers, i + 2, numbers, i + 1, numbers.Length - i - 2);
                    Array.Copy(operations, i + 1, operations, i, operations.Length - i - 1);
                    Array.Resize(ref numbers, numbers.Length - 1);
                    Array.Resize(ref operations, operations.Length - 1);
                    i--;
                }
            }
        }

        damage = numbers[0];
        if (numOfOperand > 1) {
            for (int i = 1; i < numbers.Length; i++) {
                if (operations[i - 1] == '+') {
                    damage += numbers[i];
                } else if (operations[i - 1] == '-') {
                    damage -= numbers[i];
                }
            }
        }
        foreach(Collider2D enemy in hitEmenies){
            if(enemy.GetComponent<Health>().SetHealth(damage,pureDamage) == true) gun.kills++;
            gun.echo += damage;
        } 
    }
    void BulletDisappear()
    {
        if (transform.childCount > 0) {
            GameObject effect = Instantiate(Effect, transform.GetChild(0));
            ParticleSystem particle = effect.GetComponent<ParticleSystem>();

            var mainModule = particle.main;
            mainModule.startColor = GetComponent<SpriteRenderer>().color;

            effects.transform.parent = null;

            Destroy(effects, 1);

            if (bulletStyle == 1) AreaTakeDamage();

            Destroy(gameObject);
        } 
    }
}
