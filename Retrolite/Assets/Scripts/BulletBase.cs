using UnityEngine;
using CalculatingSystem;
using System.Collections;

public class BulletBase : MonoBehaviour
{
    protected BulletData data;
    protected GunBase gun;
    protected Rigidbody2D rb;
    protected FormulaContext context;
    protected float time = 0;
    protected Coroutine lifeCoroutine;

    public bool Destroyed { get; private set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(GunBase Gun, BulletData Data, FormulaContext Context)
    {
        gun = Gun;
        data = Data;
        context = Context;
        DestroyBullet();
    }

    public virtual void Fire()
    {
        rb.simulated = true;
        rb.linearVelocity = transform.up * data.Speed.Evaluate(context);

        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        lifeCoroutine = StartCoroutine(LifeTimer());

        transform.parent = null;
        time = Time.time;
        Destroyed = false;
        context.Bullet = this;
        transform.localScale = Vector3.one * data.Scale.Evaluate(context);
    }

    protected IEnumerator LifeTimer()
    {
        float time = data.LifeTime.Evaluate(context);

        if (data.IsDynamic)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                context.Bullet = this;
                transform.localScale = Vector3.one * data.Scale.Evaluate(context);
                yield return null;
            }
        }
        else yield return new WaitForSeconds(time);

        context.Echo = 0;
        DestroyBullet();
    }

    protected void DestroyBullet()
    {
        rb.simulated = false;
        gameObject.SetActive(false);
        Destroyed = true;
        transform.parent = gun.transform;
        transform.position = gun.transform.position;
        transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            context.Bullet = this;
            context.Health = other.GetComponent<HealthBase>();
            float damage = data.Damage.Evaluate(context);
            context.Health.TakeDamage(damage);
            context.Echo = damage;
            DestroyBullet();
        }
        else context.Echo = 0;
    }

    public float GetLifetime() => Time.time - time;
}

public struct BulletData
{
    // Static stats
    public FormulaNode Damage;
    public FormulaNode LifeTime;

    // Dynamic stats
    public FormulaNode Scale;
    public FormulaNode Speed;
    //public FormulaNode Angle;

    public bool IsDynamic;

    public BulletData(float speed = 5, float damage = 10, float lifeTime = 3, float scale = 1)
    {
        Speed = new ConstantNode(speed);
        Damage = new ConstantNode(damage);
        LifeTime = new ConstantNode(lifeTime);
        Scale = new ConstantNode(scale);

        IsDynamic = false;
    }
}
