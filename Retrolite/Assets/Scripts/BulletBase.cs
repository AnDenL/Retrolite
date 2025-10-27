using UnityEngine;
using CalculatingSystem;
using System.Collections;
using Creatures;

public class BulletBase : MonoBehaviour
{
    protected BulletPool pool;
    protected BulletData data;

    protected FormulaContext context;
    public FormulaContext Context => context;

    protected float time;
    protected Coroutine lifeCoroutine;
    protected Vector2 start;
    protected SpriteRenderer projectileRenderer;

    protected Color color;
    protected float life;
    protected bool handleDestroy;

    public Alignment OwnerAlignment { get; private set; }

    public float Spread { get; protected set; }
    public float Speed { get; protected set; }
    public float Angle { get; protected set; }
    public float Scale { get; protected set; }

    public bool Inactive { get; protected set; }

    public virtual void Initialize(BulletData Data, FormulaContext Context, BulletPool Pool)
    {
        pool = Pool;
        data = Data;
        context = Context;

        context.Bullet = this;
        OwnerAlignment = context.Owner.Alignment;

        projectileRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Fire(float spread)
    {
        gameObject.SetActive(true);
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        Spread = spread;
        lifeCoroutine = StartCoroutine(LifeTimer());

        Angle = transform.rotation.eulerAngles.z;
        start = (Vector2)transform.position;
        transform.parent = null;
        time = Time.time;
        Inactive = false;

        Speed = data.Speed.Evaluate(context);
        Scale = data.Scale.Evaluate(context);

        transform.localScale = Vector3.one * Scale;
        float formulaAngle = data.Angle.Evaluate(context);
        float angle = spread + (Angle + (formulaAngle * Mathf.Rad2Deg));
        Angle -= formulaAngle + Speed < 0 ? 180 : 0;

        if (float.IsNaN(angle) || float.IsInfinity(angle))
            angle = 0;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        SetRendererColor();
    }

    protected virtual void Update()
    {
        if (data.IsDynamic)
        {
            if (!data.Scale.IsConstant())
            {
                Scale = Mathf.Sqrt(Mathf.Abs(data.Scale.Evaluate(context)));
                transform.localScale = Vector3.one * Scale;
            }
            if (!data.Speed.IsConstant())
                Speed = data.Speed.Evaluate(context);
            if (!data.Angle.IsConstant())
                transform.rotation = Quaternion.Euler(0, 0, Angle + (data.Angle.Evaluate(context) * Mathf.Rad2Deg));

            SetRendererColor();
        }

        transform.position += Speed * Time.deltaTime * 2 * transform.up;
    }

    protected IEnumerator LifeTimer()
    {
        life = data.LifeTime.Evaluate(context);
        yield return new WaitForSeconds(life);
        Deactivate();
    }

    public void HandleProjectileDestroy()
    {
        if (Inactive) Destroy(gameObject);
        handleDestroy = true;
    }

    protected virtual void Deactivate()
    {
        if (handleDestroy) Destroy(gameObject);
        Inactive = true;

        pool.Return(this);

        transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    protected virtual void SetRendererColor()
    {
        float r = data.Damage.Evaluate(context) / 5 + (context.Owner.IsEnemyTo(PlayerController.Player) ? 0 : 5);
        float g = life / 3;
        float b = Speed / 5;
        color = new Color(
            Mathf.Clamp(r, 0, 5),
            Mathf.Clamp(g, 0, 5),
            Mathf.Clamp(b, 0, 5),
            1 / Mathf.Clamp(Scale, 1, 5)
        );
        projectileRenderer.color = color;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        if (other.TryGetComponent(out Creature creature))
        {
            if (!creature.IsEnemyTo(context.Owner)) return;

            context.TargetCreature = creature;
            context.TargetHealth = creature.HealthComponent;
            context.TargetCreature.StartKnockback(
            data.Knockback.Evaluate(context) / 10,
            transform.up
        );
        }
        else
        {
            if (!other.TryGetComponent(out HealthBase health))
            {
                Deactivate();
                return;
            }

            context.TargetCreature = null;
        }

        float damage = data.Damage.Evaluate(context);
        context.TargetHealth.TakeDamage(damage, context);

        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        Deactivate();
    }

    // Helpers
    public float GetLifetime() => Time.time - time;
    public float GetDestroyTime() => life - (Time.time - time);
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
    [SerializeReference]
    public FormulaNode Knockback;

    // Dynamic stats
    [SerializeReference]
    public FormulaNode Scale;
    [SerializeReference]
    public FormulaNode Speed;
    [SerializeReference]
    public FormulaNode Angle;

    public bool IsDynamic;

    public BulletData(float speed = 8, float damage = 10, float lifeTime = 3, float scale = 1, float angle = 0, float knockback = 1)
    {
        Speed = new ConstantNode(speed);
        Damage = new ConstantNode(damage);
        LifeTime = new ConstantNode(lifeTime);
        Scale = new ConstantNode(scale);
        Angle = new ConstantNode(angle);
        Knockback = new ConstantNode(knockback);

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }

    public void GenerateRandomFormulas()
    {
        Speed = FormulaGenerator.GenerateRandomFormula();
        Damage = FormulaGenerator.GenerateRandomFormula();
        LifeTime = FormulaGenerator.GenerateRandomFormula();
        Scale = FormulaGenerator.GenerateRandomFormula();
        FormulaNode tempAngle = FormulaGenerator.GenerateRandomFormula();
        Angle = tempAngle.IsConstant() ? new ConstantNode(0) : tempAngle;
        Knockback = FormulaGenerator.GenerateRandomFormula();

        Debug.Log($"Speed: {Speed.ToReadableString()}");
        Debug.Log($"Damage: {Damage.ToReadableString()}");
        Debug.Log($"LifeTime: {LifeTime.ToReadableString()}");
        Debug.Log($"Scale: {Scale.ToReadableString()}");
        Debug.Log($"Angle: {Angle.ToReadableString()}");
        Debug.Log($"Knockback: {Knockback.ToReadableString()}");

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }
}
