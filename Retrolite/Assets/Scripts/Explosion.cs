using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _radius;
    [SerializeField] private float _knockback;
    [SerializeField] private LayerMask _layers;

    private void Explode()
    {
        Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position, _radius, _layers);

        foreach(Collider2D enemy in enemys)
        {
            HealthBase hp = enemy.GetComponent<HealthBase>();

            if(hp != null)
            {
                hp.SetHealth(_damage - Vector2.Distance(enemy.transform.position,transform.position) / _radius);
            }

            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = enemy.transform.position - transform.position;
                direction.Normalize();
                rb.AddForce(direction * _knockback, ForceMode2D.Impulse);
            }
        }
    }
}
