using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemHead : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform rotate, target;
    public float minAttackTime, maxAttackTime;
    public int charges = 0;
    public GameObject Body;
    public GameObject laser;
    public SpriteRenderer sphere_1, sphere_2,sphere_3;
    public AudioClip laserSound;

    private AudioSource sound;
    private Health hp;
    private float ChargeTime;
    private Bounce bounce;
    private GameObject bullet;
    private float attackTime = 3f;

    private void Start()
    {
        hp = Body.GetComponent<Health>();
        sound = GetComponent<AudioSource>();
    }
    private void Attack()
    {
        attackTime = Time.time + Random.Range(minAttackTime, maxAttackTime);
        float r = Random.Range(-30f,30f);
        rotate.localRotation = Quaternion.Euler(0, 0, r);

        for (int i = 0;i < 6;i++)
        {
            rotate.localRotation = Quaternion.Euler(0, 0,r + (60*i));
            bullet = Instantiate(bulletPrefab, transform.parent);
            bullet.transform.position = transform.position;
            bounce = bullet.GetComponent<Bounce>();
            bounce.directionX = transform.position.x - target.position.x;
            bounce.directionY = transform.position.y - target.position.y;
        }
    }

    public void LaserShoot()
    {
        charges--;

        sound.pitch = Random.Range(0.8f,1.2f);
        sound.PlayOneShot(laserSound);

        if(charges > 0)sphere_1.color = new Color(1,1,1,1);
        else sphere_1.color = new Color(0.1f,0.1f,0.1f,1);
        if(charges > 1)sphere_2.color = new Color(1,1,1,1);
        else sphere_2.color = new Color(0.1f,0.1f,0.1f,1);
        if(charges > 2)sphere_3.color = new Color(1,1,1,1);
        else sphere_3.color = new Color(0.1f,0.1f,0.1f,1);

        Instantiate(laser, transform.GetChild(0));
    }

    private void Update()
    {
        if (Time.time >= attackTime) Attack();
        if(ChargeTime < Time.time)
        {
            if(charges < 3)charges ++;
            else hp.Heal(3);

            if(charges > 0)sphere_1.color = new Color(1,1,1,1);
            else sphere_1.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 1)sphere_2.color = new Color(1,1,1,1);
            else sphere_2.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 2)sphere_3.color = new Color(1,1,1,1);
            else sphere_3.color = new Color(0.1f,0.1f,0.1f,1);

            ChargeTime = Time.time + 4;
        }
    }
    private void Death()
    {
        Body.GetComponent<GolemBody>().charges = 0;
        hp.SetHealth(hp.healthPoint);
    }
}
