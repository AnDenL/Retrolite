using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public WeaponBase Weapon;
    public string Damage;
    public int Number;
    public float Speed;
    public Vector2 StartPosition;
    public HealthBase Health;
    private Rigidbody2D rb;  
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void CreateBullet(int number, string damage, WeaponBase weapon)
    {
        Damage = damage;
        Weapon = weapon;
        Number = number;
    }
    public void Shoot(float force, float range)
    {
        if(force > 100) force = 100;
        Speed = force;
        gameObject.SetActive(true);
        rb.velocity = Vector2.zero;
        rb.simulated = true;
        transform.parent = null;
        StartPosition = new Vector2(transform.position.x, transform.position.y);
        rb.AddForce(transform.right * force, ForceMode2D.Impulse);
        Invoke("Disapear", Mathf.Abs(range / force) * 2);
    }
    public void Return(Transform magazine)
    {
        rb.simulated = false;
        transform.rotation = magazine.rotation;
        transform.parent = magazine;
        transform.position = magazine.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player"))
        {
            Disapear();
            Health = other.GetComponent<HealthBase>();
            if(Health != null) 
            {
                float value = Game.Calculate(Damage, this);
                Weapon.EchoDamage = value;
                Health.SetHealth(value);
            }
            else
                Weapon.EchoDamage = 0;
        }
    }

    private void Disapear()
    {
        gameObject.SetActive(false);

    }
}
