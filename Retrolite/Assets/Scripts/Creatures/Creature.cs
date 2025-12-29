using System;
using Creatures;
using UnityEngine;
using System.Collections.Generic;
using static Creatures.Alignment;
using System.Collections;
using CalculatingSystem;

[RequireComponent(typeof(HealthBase))]
[RequireComponent(typeof(CorruptibleBase))]
public class Creature : MonoBehaviour, IDamagable, ICorruptible
{
    #region Fields and Properties

    [Header("Creature")]
    [SerializeField] protected AIController controller;
    [SerializeField] protected float visionRange = 8f;

    public AIController Controller => controller;
    public float VisionRange => visionRange;

    [SerializeField] protected List<Skill> skillTemplates = new();
    [SerializeField] protected List<PassiveSkill> passiveTemplates = new();

    protected readonly List<Skill> activeSkills = new();
    protected readonly List<PassiveSkill> passiveSkills = new();

    public IReadOnlyList<Skill> ActiveSkills => activeSkills;
    public IReadOnlyList<PassiveSkill> PassiveSkills => passiveSkills;

    protected ResourceContainer resources;
    public ResourceContainer Resources => resources;

    protected List<Effect> effects = new();

    [Header("Movement")]
    public float Speed = 5f;
    [SerializeField] protected DirectionSkill baseMovementSkill;
    public DirectionSkill BaseMovement => baseMovementSkill;

    public bool CanAct = true;

    public Creature Target => controller.Target;
    public Alignment Alignment => controller.Alignment;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public HealthBase HealthComponent;
    [HideInInspector] public CorruptibleBase Corruption;
    [HideInInspector] public Rigidbody2D Rb;

    protected Transform ui;
    protected int _isBackwardsHash;
    protected int _isCorruptedHash;
    protected int _corruptHash;
    protected int _lookUpHash;
    protected int _isDeadHash;
    
    public bool FacingRight { get; private set;}
    public Coroutine ChannelingSkill { get; private set;}

    #endregion

    #region Events

    public event Action OnUpdateAI;
    public event Action OnFixedUpdate;
    public event Action<Collision2D> CollisionStay2D;
    public event Action<Skill> OnNewSkill;
    public event Action<PassiveSkill> OnNewPassive;
    public event Action<IEnumerator> OnCast;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        HealthComponent = GetComponent<HealthBase>();
        Corruption = GetComponent<CorruptibleBase>();
        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        foreach (var template in skillTemplates)
        {
            if (template == null) continue;
            Skill instance = Instantiate(template);
            AddSkill(instance);
        }

        foreach (var template in passiveTemplates)
        {
            if (template == null) continue;
            PassiveSkill instance = Instantiate(template);
            AddPassive(instance);
        }

        if (baseMovementSkill != null)
        {
            baseMovementSkill = Instantiate(baseMovementSkill);
            baseMovementSkill.Init(this);
        }

        ui = transform.Find("UI");

        Corruption.OnCorrupting += DestabilizationAnim;
        Corruption.OnBecameVulnerable += Corrupt;
        HealthComponent.OnDeath += DeathEffect;

        resources = new(this);

        controller = Instantiate(controller);
        controller.Init(this);

        _isBackwardsHash = Animator.StringToHash("IsBackwards");
        _isCorruptedHash = Animator.StringToHash("IsCorrupted");
        _isDeadHash = Animator.StringToHash("Death");
        _corruptHash = Animator.StringToHash("Corrupt");
        _lookUpHash = Animator.StringToHash("LookUp");
    }

    protected virtual void Update()
    {
        if (HealthComponent.IsDead) return;
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].Tick(Time.deltaTime);

            if (effects[i].IsFinished)
            {
                effects[i].OnRemove();
                effects.RemoveAt(i);
            }
        }

        if (Corruption.IsCorrupted || !CanAct) return;
        controller.UpdateAI();
        OnUpdateAI?.Invoke();
    }

    protected virtual void FixedUpdate() => OnFixedUpdate?.Invoke();

    #endregion
    #region Public Methods

    public void AddEffect(Effect effect)
    {
        Effect newEffect = Instantiate(effect);
        newEffect.Init(this);
        effects.Add(newEffect);
    }

    public void AddSkill(Skill skill)
    {
        activeSkills.Add(skill);
        skill.Init(this);
        OnNewSkill?.Invoke(skill);
    }

    public void AddPassive(PassiveSkill passive)
    {
        passiveSkills.Add(passive);
        passive.Init(this);
        OnNewPassive?.Invoke(passive);
    }

    public virtual void LookAt(Vector3 position)
    {
        if (position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if (ui) ui.localScale = new Vector3(-1, 1, 1);
            FacingRight = true;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (ui) ui.localScale = new Vector3(1, 1, 1);
            FacingRight = false;
        }

        Animator.SetBool(_lookUpHash, position.y > transform.position.y + 1.5f);
    }

    public void UpdateAnimationState()
    {
        if (Rb.velocity.sqrMagnitude <= 0.001f) return;

        bool movingLeft = Rb.velocity.x < 0f;
        Animator.SetBool(_isBackwardsHash, movingLeft != FacingRight);
    }

    public bool IsEnemyTo(Creature other)
    {
        if (other == null || other == this) return false;

        return Alignment switch
        {
            Ally => other.Alignment is Enemy or EvilEnemy,
            EvilAlly => !(other.Alignment is Ally or EvilAlly),
            Neutral => other.Alignment is EvilEnemy or EvilAlly or Evil,
            Evil => true,
            Enemy => other.Alignment is Ally or EvilAlly,
            EvilEnemy => !(other.Alignment is Enemy or EvilEnemy),
            FullyFriendly => false,
            _ => false
        };
    }

    public virtual Creature FindTarget()
    {
        LayerMask obstacleMask = LayerMask.GetMask("Walls");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange);

        Creature bestTarget = null;
        float bestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Creature creature))
            {
                if (creature == this) continue;
                if (!creature.IsEnemyTo(this)) continue;
                if (creature.HealthComponent.IsDead) continue;

                Vector2 dir = (creature.transform.position - transform.position).normalized;
                float dist = Vector2.Distance(transform.position, creature.transform.position);

                RaycastHit2D block = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
                if (block.collider != null) continue;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = creature;
                }
            }
        }

        return bestTarget;
    }

    public virtual bool Cast()
    {
        OnCast?.Invoke(null);
        return true;
    }

    public virtual bool Cast(IEnumerator enumerator)
    {
        OnCast?.Invoke(enumerator);
        bool notCasting = ChannelingSkill == null;
        if (notCasting) ChannelingSkill = StartCoroutine(CastWrapper(enumerator));
        
        return notCasting;
    }

    public virtual bool Break()
    {
        bool isCasting = ChannelingSkill != null;
        if (isCasting) StopCoroutine(ChannelingSkill);
        ChannelingSkill = null;

        return isCasting;
    }

    IEnumerator CastWrapper(IEnumerator routine)
    {
        yield return routine;
        ChannelingSkill = null;
    }

    public void ApplyCorruption(int amount, Creature source) => Corruption.ApplyCorruption(amount, source);
    public void Redact() => Corruption.Redact();

    public virtual void Heal(float value) => HealthComponent.Heal(value);
    public virtual void TakeDamage(float value) => HealthComponent.TakeDamage(value);
    public virtual void TakeDamage(float value, Context context) => HealthComponent.TakeDamage(value, context);
    public virtual void Knockback(Vector2 dir, float strength) => Rb.AddForce(dir * -strength, ForceMode2D.Impulse);

    #endregion
    #region Private Methods

    protected void Corrupt()
    {
        HealthComponent.IsWeak = true;
        Animator.SetBool(_isCorruptedHash, true);
    }

    protected void OnCollisionStay2D(Collision2D collision) => CollisionStay2D?.Invoke(collision);
    protected void DestabilizationAnim(int i) => Animator.SetTrigger(_corruptHash);
    protected void DeathEffect() => Animator.SetBool(_isDeadHash, true);

    #endregion
}
