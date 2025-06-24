using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    [SerializeField]
    protected float speed = 5f;
    [SerializeField]
    protected float damage = 10f;
    [SerializeField]
    protected float lifetime = 5f;

    public virtual void Shoot(Vector2 direction)
    {
        Invoke("DestroyBullet", lifetime);
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
    }

    protected virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hit(other);
        }
    }

    protected virtual void Hit(Collider2D other)
    {
        other.GetComponent<HealthBase>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
