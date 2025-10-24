using UnityEngine;
using System.Collections.Generic;
using CreatureAI;
using static CreatureAI.Alignment;
using System;

[RequireComponent(typeof(HealthBase))]
[RequireComponent(typeof(Corruptible))]
public class Creature : MonoBehaviour
{
    [Header("Creature")]

    [SerializeField] private AIController controller;
    
    [SerializeField] protected float visionRange;
    public float VisionRange => visionRange;

    [SerializeField] private List<Skill> skillTemplates = new();
    [SerializeField] private List<PassiveSkill> passiveTemplates = new();
    [SerializeField] private DirectionSkill baseMovementSkill;

    private readonly List<Skill> activeSkills = new();
    private readonly List<PassiveSkill> passiveSkills = new();

    public IReadOnlyList<Skill> ActiveSkills => activeSkills;
    public IReadOnlyList<PassiveSkill> PassiveSkills => passiveSkills;


    [Header("Movement")]

    [SerializeField] protected float speed = 5;
    public float Speed => speed;
    public DirectionSkill BaseMovement { get; private set; }

    public Creature Target => controller.Target;
    public Alignment Alignment => controller.Alignment;

    public event Action<Collision2D> CollisionEnter2D;
    public event Action<Collision2D> CollisionStay2D;

    public event Action<Skill> OnNewSkill;
    public event Action<PassiveSkill> OnNewPassive;

    [HideInInspector] public Animator Animator;
    [HideInInspector] public HealthBase HealthComponent;
    [HideInInspector] public Corruptible Corruption;
    [HideInInspector] public Rigidbody2D Rb;

    private Transform ui;

    private int _isBackwards;

    private bool facingLeft;

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

        BaseMovement = Instantiate(baseMovementSkill);
        BaseMovement.Init(this);

        ui = transform.Find("UI");

        if (Corruption != null)
        {
            Corruption.OnCorrupting += DestabilizationAnim;
            Corruption.OnBecameVulnerable += Corrupt;
        }
        HealthComponent.OnDeath += DeathEffect;
        _isBackwards = Animator.StringToHash("IsBackwards");
    }

    protected virtual void Update()
    {
        if (HealthComponent.IsDead || Corruption.isCorrupted) return;
        controller.UpdateAI();
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
        passive.Subscribe(this);
        OnNewPassive?.Invoke(passive);
    }

    public void LookAt(Vector3 position)
    {
        if (position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            ui.localScale = new Vector3(1, 1, 1);
            facingLeft = true;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            ui.localScale = new Vector3(-1, 1, 1);
            facingLeft = false;
        }
    }

    public void UpdateAnimationState()
    {
        if (Rb.velocity.sqrMagnitude > 0.001f)
        {
            bool movingLeft = Rb.velocity.x < 0f;
            bool isBackwards = movingLeft != facingLeft;

            Animator.SetBool(_isBackwards, isBackwards);
        }
    }

    private void DeathEffect()
    {
        Animator.SetTrigger("Death");
    }

    private void Corrupt()
    {
        HealthComponent.IsWeak = true;
        Animator.SetBool("IsCorrupted", true);
    }

    private void DestabilizationAnim(int i)
    {
        Animator.SetTrigger("Corrupt");
    }

    public void StartKnockback(float strength, Vector2 dir)
    {
        dir.Normalize();
        Rb.velocity = Vector2.zero;
        Rb.AddForce(strength * dir, ForceMode2D.Impulse);
    }

    public bool IsEnemyTo(Creature other)
    {
        if (other == null) return false;
        if (other == this) return false;

        switch (Alignment)
        {
            default:
                return false;
            case Ally:
                return other.Alignment == Enemy || other.Alignment == EvilEnemy;
            case EvilAlly:
                return !(other.Alignment == Ally || other.Alignment == EvilAlly);
            case Neutral:
                return other.Alignment == EvilEnemy || other.Alignment == EvilAlly || other.Alignment == Evil;
            case Evil:
                return true;
            case Enemy:
                return other.Alignment == Ally || other.Alignment == EvilAlly;
            case EvilEnemy:
                return !(other.Alignment == Enemy || other.Alignment == EvilEnemy);
            case FullyFriendly:
                return false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) => CollisionEnter2D?.Invoke(collision);
    public void OnCollisionStay2D(Collision2D collision) => CollisionStay2D?.Invoke(collision);

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
}
