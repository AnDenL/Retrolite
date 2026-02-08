using UnityEngine;
using CalculatingSystem;
using System.Collections;
using Creatures;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class BulletBase : MonoBehaviour
{
    protected BulletData data;

    protected BulletPool pool;
    
    protected Context context;
    public Context Context => context;

    protected float time;
    protected Coroutine lifeCoroutine;
    protected Vector2 start;
    protected SpriteRenderer projectileRenderer;
    protected AudioSource source;

    protected Color color;
    protected float life;
    protected bool handleDestroy;

    public Alignment OwnerAlignment { get; private set; }

    public float Spread { get; protected set; }
    public float Speed { get; protected set; }
    public float Angle { get; protected set; }
    public float Scale { get; protected set; }

    public int Number { get; protected set; }

    public bool Inactive { get; protected set; }

    public virtual void Initialize(BulletData Data, Context Context, BulletPool Pool)
    {
        pool = Pool;
        data = Data;
        context = Context;

        context.Bullet = this;
        OwnerAlignment = context.Owner.Alignment;

        projectileRenderer = GetComponent<SpriteRenderer>();
        projectileRenderer.sprite = Data.BulletSprite;

        source = GetComponent<AudioSource>();
    }

    public virtual void Fire(float spread, int number)
    {
        gameObject.SetActive(true);
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        if (data.FireSound)
        {
            source.pitch = Random.Range(0.8f, 1.2f);
            source.PlayOneShot(data.FireSound);
        }
        
        Spread = spread;
        lifeCoroutine = StartCoroutine(LifeTimer());

        Number = number;

        Angle = transform.rotation.eulerAngles.z;
        start = (Vector2)transform.position;
        transform.parent = null;
        time = Time.time;
        Inactive = false;

        Speed = data.Speed.Evaluate(context);
        Scale = Mathf.Max(0.1f, Mathf.Sqrt(Mathf.Abs(data.Scale.Evaluate(context))));

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
                Scale = Mathf.Max(0.1f, Mathf.Sqrt(Mathf.Abs(data.Scale.Evaluate(context))));
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
        life = Mathf.Abs(data.LifeTime.Evaluate(context));
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
        context.Position = transform.position;
        data.OnReturn?.Execute(context);
        Inactive = true;

        pool.Return(this);

        transform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    protected virtual void SetRendererColor()
    {
        float r = data.Damage.Evaluate(context) / 5;
        float g = life / 3;
        float b = Speed / 5;
        color = context.Owner.IsEnemyTo(PlayerController.Player) ? Color.red 
        : new Color(
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

            context.Target = creature;
            creature.Rb.AddForce(data.Knockback.Evaluate(context) / 10 * transform.up, ForceMode2D.Impulse);
            data.OnDamage?.Execute(context);
        }
        else
        {
            if (!other.TryGetComponent(out HealthBase health))
            {
                Deactivate();
                return;
            }

            context.Target = null;
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
    public void ApplyCorruption(int value) => Deactivate();
    public void Redact() {}
    public void Knockback(Vector2 dir, float strength) {}
}


[System.Serializable]
public class BulletData
{
    // Static stats
    public Formula Damage;
    public Formula LifeTime;
    public Formula Knockback;

    // Dynamic stats
    public Formula Scale;
    public Formula Speed;
    public Formula Angle;

    [Header("Actions")]
    [SerializeReference]
    public ActionNode OnReturn;
    [SerializeReference]
    public ActionNode OnDamage;

    public Sprite BulletSprite;
    public AudioClip FireSound;

    public bool IsDynamic;

    public BulletData(float speed = 8, float damage = 10, float lifeTime = 3, float scale = 1, float angle = 0, float knockback = 1)
    {
        Speed = new Formula(new ConstantNode(speed));
        Damage = new Formula(new ConstantNode(damage));
        LifeTime = new Formula(new ConstantNode(lifeTime));
        Scale = new Formula(new ConstantNode(scale));
        Angle = new Formula(new ConstantNode(angle));
        Knockback = new Formula(new ConstantNode(knockback));
        BulletSprite = WeaponGenerator.Instance.BulletList.RandomSprite();

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }

    public void GenerateRandomFormulas(GameRandom rnd)
    {
        Speed = new Formula(FormulaGenerator.GenerateRandomFormula(rnd));
        Damage = new Formula(FormulaGenerator.GenerateRandomFormula(rnd));
        LifeTime = new Formula(FormulaGenerator.GenerateRandomFormula(rnd));
        Scale = new Formula(FormulaGenerator.GenerateRandomFormula(rnd));
        FormulaNode tempAngle = FormulaGenerator.GenerateRandomFormula(rnd);
        Angle = new Formula(tempAngle.IsConstant() ? new ConstantNode(0) : tempAngle);
        Knockback = new Formula(FormulaGenerator.GenerateRandomFormula(rnd));

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }
}
