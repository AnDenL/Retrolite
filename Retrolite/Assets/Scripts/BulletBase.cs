using UnityEngine;
using CalculatingSystem;
using System.Collections;

public class BulletBase : MonoBehaviour
{
    protected BulletData data;
    protected GunBase gun;
    protected FormulaContext context;
    protected float time = 0;
    protected Coroutine lifeCoroutine;
    protected Vector2 start;
    protected SpriteRenderer bulletRenderer;

    protected Color color;
    protected float life;

    public float spread { get; protected set; }
    public float speed { get; protected set; }
    public float angle { get; protected set; }
    public float scale { get; protected set; }

    public bool Destroyed { get; protected set; }

    public void Initialize(GunBase Gun, BulletData Data, FormulaContext Context)
    {
        gun = Gun;
        data = Data;
        context = Context;
        DestroyBullet();
        context.Bullet = this;
        context.Gun = gun;
    }

    public virtual void Fire(float spread)
    {
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        this.spread = spread;
        lifeCoroutine = StartCoroutine(LifeTimer());

        angle = transform.rotation.eulerAngles.z;
        start = (Vector2)transform.position;
        transform.parent = null;
        time = Time.time;
        Destroyed = false;
        speed = data.Speed.Evaluate(context);
        scale = data.Scale.Evaluate(context);
        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.Euler(0, 0, spread + (angle + (data.Angle.Evaluate(context) * Mathf.Rad2Deg)));
        SetRendererColor();
    }

    protected void Update()
    {
        if (data.IsDynamic)
        {
            if (!(data.Scale.IsConstant()))
            {
                scale = data.Scale.Evaluate(context);
                transform.localScale = Vector3.one * scale;
            }
            if (!(data.Speed.IsConstant())) speed = data.Speed.Evaluate(context);
            if (!(data.Angle.IsConstant())) transform.rotation = Quaternion.Euler(0, 0, angle + (data.Angle.Evaluate(context) * Mathf.Rad2Deg));
            SetRendererColor();
        }
        transform.position += transform.up * speed * Time.deltaTime;
    }

    protected IEnumerator LifeTimer()
    {
        life = data.LifeTime.Evaluate(context);

        yield return new WaitForSeconds(life);

        gun.Data.Echo = 0;
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

    protected void SetRendererColor()
    {
        float r = data.Damage.Evaluate(context) / 10;
        float g = data.LifeTime.Evaluate(context) / 3;
        float b = data.Speed.Evaluate(context) / 5;
        color = new Color(Mathf.Clamp(r, 0, 5), Mathf.Clamp(g, 0, 5), Mathf.Clamp(b, 0, 5), 1);
        bulletRenderer = GetComponent<SpriteRenderer>();
        bulletRenderer.color = color;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        if (other.CompareTag("Enemy"))
        {
            context.Health = other.GetComponent<HealthBase>();
            float damage = data.Damage.Evaluate(context);
            context.Health.TakeDamage(damage);
            gun.Data.Echo = damage;
            if (lifeCoroutine != null)
                StopCoroutine(lifeCoroutine);
            DestroyBullet();
        }
        else
        {
            gun.Data.Echo = 0;
            DestroyBullet();
        }
    }

    public float GetLifetime() => Time.time - time;
    public float GetDestroyTime() => life - (Time.time - time);
    public float GetDistanceTravelled() => Vector2.Distance(start, transform.position);
}

[System.Serializable]
public struct BulletData
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

    public BulletData(float speed = 5, float damage = 10, float lifeTime = 3, float scale = 1, float angle = 0)
    {
        Speed = new ConstantNode(speed);
        Damage = new ConstantNode(damage);
        LifeTime = new ConstantNode(lifeTime);
        Scale = new ConstantNode(scale);
        Angle = new ConstantNode(angle);

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }
}
