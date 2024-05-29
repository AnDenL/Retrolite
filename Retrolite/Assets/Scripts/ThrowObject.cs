using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public GameObject Effect;
    public float speed;
    public bool throwOnStart;
    public bool UseBulletColor;

    private bool throwed = false;
    private Rigidbody2D Rigidbody;
    private Vector3 direction;
    private float rotationZ;

    private void Start()
    {
        if(throwOnStart) 
        {
            Transform t = GameObject.FindGameObjectWithTag("Player").transform;
            Throw(new Vector3(t.position.x,t.position.y,0));
        }
    }

    public void Throw(Vector3 Target)
    {
        throwed = true;
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.constraints = RigidbodyConstraints2D.None;
        transform.parent = null;
        direction = Target - transform.position;
        rotationZ = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, rotationZ);
        Rigidbody.AddForce(transform.right * speed, ForceMode2D.Impulse);
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(throwed)
        {
            if (collision.gameObject.tag == "Player")
            {
                Health health = collision.gameObject.GetComponent<Health>();
                health.SetHealth(4);
                BulletDisappear();
            }
            else if(collision.gameObject.tag == "Tilemap") BulletDisappear();
        }
    }
    private void BulletDisappear()
    {
        Rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        GameObject effects = Instantiate(Effect, transform);
        if(UseBulletColor)
        {
            ParticleSystem particle = effects.GetComponent<ParticleSystem>();
            var mainModule = particle.main;
            mainModule.startColor = GetComponent<SpriteRenderer>().color;
        }

        effects.transform.parent = null;
        Health hp = gameObject.GetComponent<Health>();
        Destroy(effects, 1);
        if(hp != null) hp.SetHealth(hp.healthPoint);
        else Destroy(gameObject);
    }
}
