using System;
using Creatures;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CalculatingSystem;

[RequireComponent(typeof(HealthBase))]
[RequireComponent(typeof(CorruptibleBase))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Creature : MonoBehaviour, IDamagable, ICorruptible
{
    #region Fields and Properties

    [Header("Creature")]
    [SerializeField] protected AIController controller;
    public float VisionRange = 8f;

    protected Inventory inventory;
    public Inventory Inventory => inventory;

    public AIController Controller => controller;
    public Alignment AlignmentEditable { get => controller.Alignment; set => controller.Alignment = value; }
    public float VisionEditable{ get => VisionRange; set => VisionRange = value; }

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
    protected float SpeedEditable{ get => Speed; set => Speed = value; }
    [SerializeField] protected DirectionSkill baseMovementSkill;
    public DirectionSkill BaseMovement => baseMovementSkill;

    public bool CanAct = true;

    public Creature Target => controller.Target;
    public Alignment Alignment => controller.Alignment;

    protected bool isActive = true;

    public virtual bool IsActive 
    {
        get => isActive;
        set
        {
            if (isActive == value) return;
            isActive = value;
            OnActiveStateChanged(value);
        }
    }

    [SerializeField] protected Transform ui;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public HealthBase HealthComponent;
    [HideInInspector] public CorruptibleBase Corruption;
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public AudioSource Source;

    protected int _isBackwardsHash;
    protected int _isCorruptedHash;
    protected int _corruptHash;
    protected int _lookUpHash;
    protected int _isDeadHash;
    
    public bool FacingRight { get; private set;}
    public Coroutine ChannelingSkill { get; private set;}

    private static readonly Collider2D[] targetSearchBuffer = new Collider2D[16];
    private LayerMask creatureLayerMask;
    private LayerMask wallsLayerMask;

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
        Source = GetComponent<AudioSource>();

        inventory = new();

        creatureLayerMask = LayerMask.GetMask("Creature");
        wallsLayerMask = LayerMask.GetMask("Walls");

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

        ui = ui ? ui : transform.Find("UI");

        Corruption.OnCorrupting += DestabilizationAnim;
        Corruption.OnVulnerabilityChange += Corrupt;
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

    protected virtual void Start()
    {
        if (CreaturesManager.Instance != null)
        {
            CreaturesManager.RegisterCreature(this);
        }
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

    public int AddItem(Item item, int count = 1) => inventory.AddItem(item, count);
    public int AddItem(ItemStack stack) => inventory.AddItem(stack.Item, stack.Count);
    public bool RemoveItem(Item item, int count = 1) => inventory.RemoveItem(item, count);
    public int GetItemCount(Item item) => inventory.GetItemCount(item);

    public void AddEffect(Effect effect)
    {
        Effect newEffect = Instantiate(effect);
        newEffect.Init(this);
        effects.Add(newEffect);
    }

    public void AddEffect(Effect effect, float strength, float duration)
    {
        Effect newEffect = Instantiate(effect);
        newEffect.Strenght = strength;
        newEffect.Duration = duration;
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
        if (!isActive) return;

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
        if (!isActive) return;
        if (Rb.velocity.sqrMagnitude <= 0.001f) return;

        bool movingLeft = Rb.velocity.x < 0f;
        Animator.SetBool(_isBackwardsHash, movingLeft != FacingRight);
    }

    public bool IsEnemyTo(Creature other)
    {
        if (other == null || other == this) return false;
        return AlignmentSystem.IsEnemy(Alignment, other.Alignment);
    }

    public virtual Creature FindTarget()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, VisionRange, targetSearchBuffer, creatureLayerMask);

        Creature bestTarget = null;
        float bestDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = targetSearchBuffer[i];
            
            if (hit.TryGetComponent(out Creature creature))
            {
                if (creature == this) continue;
                if (!creature.IsEnemyTo(this)) continue;
                if (creature.HealthComponent.IsDead) continue;

                float dist = Vector2.Distance(myPos, creature.transform.position);
                if (dist >= bestDist) continue;

                Vector2 dir = (creature.transform.position - myPos).normalized;
                RaycastHit2D block = Physics2D.Raycast(myPos, dir, dist, wallsLayerMask);
                
                if (block.collider == null)
                {
                    bestDist = dist;
                    bestTarget = creature;
                }
            }
        }
        Array.Clear(targetSearchBuffer, 0, count); 

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

    public void PlaySound(AudioClip clip)
    {
        if(!isActive) return;
        Source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        Source.PlayOneShot(clip);
    }

    public virtual void Heal(float value) => HealthComponent.Heal(value);
    public virtual void TakeDamage(float value) => HealthComponent.TakeDamage(value);
    public virtual void TakeDamage(float value, Context context) => HealthComponent.TakeDamage(value, context);
    public virtual void Knockback(Vector2 dir, float strength) => Rb.AddForce(dir * -strength, ForceMode2D.Impulse);

    #endregion
    #region Private Methods

    protected void Corrupt(bool b)
    {
        HealthComponent.IsWeak = b;
        Animator.SetBool(_isCorruptedHash, b);
    }

    protected void OnCollisionStay2D(Collision2D collision) => CollisionStay2D?.Invoke(collision);
    protected void DestabilizationAnim(int i) => Animator.SetTrigger(_corruptHash);
    protected void DeathEffect()
    {
        Break();
        Animator.SetBool(_isDeadHash, true);
    }

    protected void OnActiveStateChanged(bool active)
    {
        if (Animator != null) Animator.enabled = active;

        if (ui != null) ui.gameObject.SetActive(active);
    }

    #endregion
}
