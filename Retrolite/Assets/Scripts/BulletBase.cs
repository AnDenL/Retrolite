using UnityEngine;
using CalculatingSystem;
using System.Collections;

public class BulletBase : MonoBehaviour
{
    protected BulletData data;
    protected GunBase gun;
    protected FormulaContext context;
    protected float time = 0, speed = 0, angle = 0;
    protected Coroutine lifeCoroutine;
    protected Vector2 start;

    public bool Destroyed { get; private set; }

    public void Initialize(GunBase Gun, BulletData Data, FormulaContext Context)
    {
        gun = Gun;
        data = Data;
        context = Context;
        DestroyBullet();
        context.Bullet = this;
    }

    public virtual void Fire()
    {
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        lifeCoroutine = StartCoroutine(LifeTimer());

        angle = transform.rotation.eulerAngles.z;
        start = (Vector2)transform.position;
        transform.parent = null;
        time = Time.time;
        Destroyed = false;
        speed = data.Speed.Evaluate(context);
        transform.localScale = Vector3.one * data.Scale.Evaluate(context);
        transform.rotation = Quaternion.Euler(0, 0, angle + (data.Angle.Evaluate(context) * Mathf.Rad2Deg));
    }

    protected void Update()
    {
        if (!data.IsDynamic)
        {
            if (!(data.Scale.IsConstant())) transform.localScale = Vector3.one * data.Scale.Evaluate(context);
            if (!(data.Speed.IsConstant())) speed = data.Speed.Evaluate(context);
            if (!(data.Angle.IsConstant())) transform.rotation = Quaternion.Euler(0, 0, angle + (data.Angle.Evaluate(context) * Mathf.Rad2Deg));
        }
        transform.position += transform.up * speed * Time.deltaTime;
    }

    protected IEnumerator LifeTimer()
    {
        float time = data.LifeTime.Evaluate(context);

        yield return new WaitForSeconds(time);

        context.Echo = 0;
        DestroyBullet();
    }

    protected void DestroyBullet()
    {
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
            context.Health = other.GetComponent<HealthBase>();
            float damage = data.Damage.Evaluate(context);
            context.Health.TakeDamage(damage);
            context.Echo = damage;
            DestroyBullet();
        }
        else context.Echo = 0;
    }

    public float GetLifetime() => Time.time - time;
    public float GetDistanceTravelled() => Vector2.Distance(start, transform.position);
}

[System.Serializable]
public class BulletData
{
    // Static stats
    [SerializeReference]
    public FormulaNode Damage;
    [SerializeReference]
    public FormulaNode LifeTime;

    // Dynamic stats
    [SerializeReference]
    public FormulaNode Scale;
    [SerializeReference]
    public FormulaNode Speed;
    [SerializeReference]
    public FormulaNode Angle;

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
