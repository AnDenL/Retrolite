using UnityEngine;
using CalculatingSystem;
using System.Collections;
using Creatures;
using System;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;

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

    protected bool isSpeedDynamic;
    protected bool isScaleDynamic;
    protected bool isAngleDynamic;
    protected bool isColorDynamic;
    

    public virtual void Initialize(BulletData Data, Context Context, BulletPool Pool)
    {
        pool = Pool;
        data = Data;
        context = Context;

        context.Bullet = this;
        OwnerAlignment = context.Owner.Alignment;

        isSpeedDynamic = data.IsDynamic && !data.Speed.IsConstant();
        isScaleDynamic = data.IsDynamic && !data.Scale.IsConstant();
        isAngleDynamic = data.IsDynamic && !data.Angle.IsConstant();
        isColorDynamic = data.IsDynamic && (!data.Damage.IsConstant() || isSpeedDynamic);

        projectileRenderer = GetComponent<SpriteRenderer>();
        projectileRenderer.sprite = Data.BulletSprite;

        source = GetComponent<AudioSource>();
    }

    public void SetTarget(Creature t) => context.Target = t;

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

        if (Scale != float.NaN) transform.localScale = Vector3.one * Scale;
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
        if (Inactive) return;

        if (data.IsDynamic)
        {
            if (isScaleDynamic)
            {
                Scale = Mathf.Clamp(Mathf.Sqrt(Mathf.Abs(data.Scale.Evaluate(context))),0.1f, 100);
                transform.localScale = Vector3.one * Scale;
            }
            if (isSpeedDynamic)
            {
                float s = data.Speed.Evaluate(context);
                if (!float.IsNaN(s) && !float.IsInfinity(s)) Speed = s;
            }
            if (isAngleDynamic)
            {
                float a = data.Angle.Evaluate(context) * Mathf.Rad2Deg;
                if (!float.IsNaN(a) && !float.IsInfinity(a))
                    transform.rotation = Quaternion.Euler(0, 0, Angle + a);
            }
            if (isColorDynamic)
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
        if (data.OnReturn != null) data.OnReturn.Execute(context);
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
            context.Position = transform.position;
            creature.Rb.AddForce(data.Knockback.Evaluate(context) / 10 * transform.up, ForceMode2D.Impulse);
            if (data.OnDamage != null) data.OnDamage.Execute(context);
        }
        else
        {
            if (other.TryGetComponent(out HealthBase health))
            {
                health.TakeDamage(data.Damage.Evaluate(context));

                if (lifeCoroutine != null)
                    StopCoroutine(lifeCoroutine);

                Deactivate();
                return;
            }
            else
            {
                Deactivate();
                return;
            }
        }

        float damage = data.Damage.Evaluate(context);
        context.Target.HealthComponent.TakeDamage(damage, context);

        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        Deactivate();
    }

    public float GetLifetime() => Time.time - time;
    public float GetDestroyTime() => life - (Time.time - time);
    public float GetDistanceTravelled() => Vector2.Distance(start, transform.position);
    public void ApplyCorruption(int value) => Deactivate();
    public void Redact() {}
    public void Knockback(Vector2 dir, float strength) {}
}


[Serializable]
public class BulletData
{
    public Formula Damage;
    public Formula LifeTime;
    public Formula Knockback;

    public Formula Scale;
    public Formula Speed;
    public Formula Angle;

    [Header("Actions")]
    [SerializeReference] public ActionNode OnReturn;
    [SerializeReference] public ActionNode OnDamage;

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

        if (rnd.Chance(0.5f)) OnDamage = RandomAction(rnd);
        if (rnd.Chance(0.5f)) OnReturn = RandomAction(rnd);

        if (Scale.IsConstant() && Speed.IsConstant() && Angle.IsConstant()) IsDynamic = false;
        else IsDynamic = true;
    }

    private ActionNode RandomAction(GameRandom rnd) => rnd.Range(0,5) switch
    {
        0 => new ExplosionAction
        {
            Damage = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)), 
            Knockback = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)),
            Radius = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)),
            Layers = LayerMask.GetMask("Creatures")
        },
        1 => new HealAction
        {
            Amount = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)), 
            AdditionalHealth = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)),
        },
        2 => new DamageAction
        {
            Damage = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)), 
        },
        3 => new GiveResource
        {
            Amount = new Formula(FormulaGenerator.GenerateRandomFormula(rnd)),
            Resource = rnd.Range(0, 2) switch 
            {
                0 => ResourceType.Bits,
                1 => ResourceType.Money,
                2 => ResourceType.Energy,
                _ => ResourceType.Stamina,
            }
        },
        4 =>  new SpawnObjectAction
        {
            Prefab = WeaponGenerator.Instance.RandomObjects.GetRandom()
        },
        _ => null,
    };
}
