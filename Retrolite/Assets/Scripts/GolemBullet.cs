using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBullet : MonoBehaviour
{
    public GameObject Effect;
    public float bulletSpeed;

    private Rigidbody2D Rigidbody;
    private GameObject player;
    private Vector3 direction;
    private float rotationZ;

    private void Start()
    {
        transform.parent = null;
        Rigidbody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        var newPosition = player.transform.position;
        direction = newPosition - transform.position;
        rotationZ = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, rotationZ);
        Rigidbody.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Health health = collision.gameObject.GetComponent<Health>();
            health.SetHealth(2);
            BulletDisappear();
        }
        else if(collision.tag == "Tilemap")BulletDisappear();
    }

    void BulletDisappear()
    {
        GameObject effects = Instantiate(Effect, transform);
        ParticleSystem particle = effects.GetComponent<ParticleSystem>();

        var mainModule = particle.main;
        mainModule.startColor = GetComponent<SpriteRenderer>().color;

        effects.transform.parent = null;

        Destroy(effects, 1);
        Destroy(gameObject);
    }
}
