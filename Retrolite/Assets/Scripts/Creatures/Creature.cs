using System;
using Creatures;
using UnityEngine;
using System.Collections.Generic;
using static Creatures.Alignment;

[RequireComponent(typeof(HealthBase))]
[RequireComponent(typeof(Corruptible))]
public class Creature : MonoBehaviour
{
    #region Fields and Properties

    [Header("Creature")]
    [SerializeField] private AIController controller;
    [SerializeField] protected float visionRange = 8f;

    public AIController Controller => controller;
    public float VisionRange => visionRange;

    [SerializeField] private List<Skill> skillTemplates = new();
    [SerializeField] private List<PassiveSkill> passiveTemplates = new();

    private readonly List<Skill> activeSkills = new();
    private readonly List<PassiveSkill> passiveSkills = new();

    public IReadOnlyList<Skill> ActiveSkills => activeSkills;
    public IReadOnlyList<PassiveSkill> PassiveSkills => passiveSkills;

    private ResourceContainer resources;
    public ResourceContainer Resources => resources;

    [Header("Movement")]
    [SerializeField] protected float speed = 5f;
    [SerializeField] private DirectionSkill baseMovementSkill;

    public float Speed => speed;
    public DirectionSkill BaseMovement => baseMovementSkill;

    public Creature Target => controller.Target;
    public Alignment Alignment => controller.Alignment;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public HealthBase HealthComponent;
    [HideInInspector] public Corruptible Corruption;
    [HideInInspector] public Rigidbody2D Rb;

    private Transform ui;
    private int _isBackwardsHash;
    private int _isCorruptedHash;
    private int _corruptHash;
    private int _isDeadHash;
    private bool facingLeft;

    #endregion

    #region Events

    public event Action OnUpdateAI;
    public event Action OnFixedUpdate;
    public event Action<Collision2D> CollisionStay2D;
    public event Action<Skill> OnNewSkill;
    public event Action<PassiveSkill> OnNewPassive;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        HealthComponent = GetComponent<HealthBase>();
        Corruption = GetComponent<Corruptible>();
        Animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        controller = Instantiate(controller);
        controller.Init(this);

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

        baseMovementSkill = Instantiate(baseMovementSkill);
        baseMovementSkill.Init(this);

        ui = transform.Find("UI");

        Corruption.OnCorrupting += DestabilizationAnim;
        Corruption.OnBecameVulnerable += Corrupt;
        HealthComponent.OnDeath += DeathEffect;

        resources = new(this);

        _isBackwardsHash = Animator.StringToHash("IsBackwards");
        _isCorruptedHash = Animator.StringToHash("IsCorrupted");
        _isDeadHash = Animator.StringToHash("IsDead");
    }

    protected virtual void Update()
    {
        if (HealthComponent.IsDead || Corruption.isCorrupted) return;
        controller.UpdateAI();
        OnUpdateAI?.Invoke();
    }

    protected virtual void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }

    #endregion
    #region Public Methods

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

    public void LookAt(Vector3 position)
    {
        if (position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if (ui) ui.localScale = new Vector3(1, 1, 1);
            facingLeft = true;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (ui) ui.localScale = new Vector3(-1, 1, 1);
            facingLeft = false;
        }
    }

    public void UpdateAnimationState()
    {
        if (Rb.velocity.sqrMagnitude <= 0.001f) return;

        bool movingLeft = Rb.velocity.x < 0f;
        Animator.SetBool(_isBackwardsHash, movingLeft != facingLeft);
    }

    public void StartKnockback(float strength, Vector2 dir)
    {
        dir.Normalize();
        Rb.velocity = Vector2.zero;
        Rb.AddForce(strength * dir, ForceMode2D.Impulse);
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

    #endregion
    #region Private Methods

    private void Corrupt()
    {
        HealthComponent.IsWeak = true;
        Animator.SetBool(_isCorruptedHash, true);
    }

    private void OnCollisionStay2D(Collision2D collision) => CollisionStay2D?.Invoke(collision);
    private void DestabilizationAnim(int i) => Animator.SetTrigger(_corruptHash);
    private void DeathEffect() => Animator.SetTrigger(_isDeadHash);

    #endregion
}
