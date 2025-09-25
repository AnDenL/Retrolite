using System.Collections;
using CalculatingSystem;
using UnityEngine;

public class Robot : Creature
{
    /* 
    [SerializeField] private GameObject bullet;
    [SerializeField] private BulletData bulletData;
    [SerializeField] private float attackCooldown = 1;
    [SerializeField] private float speed = 3;
    [SerializeField] private float sleepDelay = 5f;

    [Header("Parts")]
    [SerializeField] private Transform UI;
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftTurbine;
    [SerializeField] private Transform rightTurbine;
    [SerializeField] private Transform clip;

    private float attackTime;
    private float dist;
    private bool isCracked;
    private bool isSleeping;
    public bool seeEnemy;
    private float lastSeenTargetTime;
    private Animator animator;

    private BulletPool pool;
    private Knockback knockback;
    private Vector2 targetPosition;

    private Vector2 headPosition;
    private Vector2 leftTurbinePosition;
    private Vector2 rightTurbinePosition;

    private void Start()
    {
        knockback = GetComponent<Knockback>();
        HealthComponent.OnDamaged += Damage;
        HealthComponent.OnDeath += Death;
        HealthComponent.Damaged += ChangeTarget;
        Corruption.OnCorrupting += DestabilizationAnim;
        Corruption.OnBecameVulnerable += Corrupt;

        var context = new FormulaContext
        {
            Owner = this
        };

        pool = new BulletPool(bullet, clip, this, bulletData, context);
        animator = GetComponent<Animator>();
        headPosition = head.localPosition;
        leftTurbinePosition = leftTurbine.localPosition;
        rightTurbinePosition = rightTurbine.localPosition;
    }

    private void Corrupt()
    {
        HealthComponent.IsWeak = true;
        animator.SetBool("IsCorrupted", true);
    }

    private void DestabilizationAnim(int i) => animator.SetTrigger("Corrupt");

    private void ChangeTarget(float v, FormulaContext context) => target = context.Owner;

    private void Damage(float t)
    {
        ParticleManager.PlayParticle(6, transform.position);

        headPosition += 0.005f * t * Random.insideUnitCircle;
        leftTurbinePosition += 0.005f * t * Random.insideUnitCircle;
        rightTurbinePosition += 0.005f * t * Random.insideUnitCircle;

        if (HealthComponent.Health < 40 && !isCracked)
        {
            StartCoroutine(AttackTimer());
            isCracked = true;
        }
    }

    private void OnDestroy()
    {
        pool.Clear();
    }

    private void Death()
    {
        isSleeping = true;
        animator.SetTrigger("Death");
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (HealthComponent.IsDead) return;
        AnimateParts();
        if (HealthComponent.IsWeak) return;
        if (target == null || target.HealthComponent.IsDead) target = FindTarget();

        if (target != null)
        {
            lastSeenTargetTime = Time.time;
            isSleeping = false;
        }
        else if (Time.time - lastSeenTargetTime > sleepDelay)
        {
            isSleeping = true;
        }

        if (isSleeping) return;

        if (target == null) return;

        dist = Vector2.Distance(transform.position, target.transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, dist, LayerMask.GetMask("Walls"));
        if (hit.collider == null) seeEnemy = true;
        else seeEnemy = false;

        if (dist < 3 && !knockback.inMoveNow)
            knockback.StartKnockback(8, HealthComponent.Health < 40 ? target.transform.position - transform.position : transform.position - target.transform.position);

        if (dist < 4)
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, -speed * Time.deltaTime);
        else if (dist > 8)
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (!seeEnemy && !knockback.inMoveNow)
            knockback.StartKnockback(5, target.transform.position - transform.position + (Vector3)Random.insideUnitCircle * 4);

        if (attackTime < Time.time && !knockback.inMoveNow) Shoot();
    }

    private void AnimateParts()
    {
        float strenght = (1 - HealthComponent.GetHealthPercent());

        head.localPosition = headPosition + 0.1f * strenght * Random.insideUnitCircle;
        leftTurbine.localPosition = leftTurbinePosition + 0.1f * strenght * Random.insideUnitCircle;
        rightTurbine.localPosition = rightTurbinePosition + 0.1f * strenght * Random.insideUnitCircle;

        if (target == null) return;

        Vector2 dir = (target.transform.position - transform.position).normalized;

        if (target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            UI.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            UI.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void Evade(Vector2 position)
    {
        if (knockback.inMoveNow) return;

        Vector2 leftDodge = new Vector2(-position.y + Random.Range(-0.25f, 0.25f), position.x + Random.Range(-0.25f, 0.25f)).normalized;
        Vector2 rightDodge = new Vector2(position.y + Random.Range(-0.25f, 0.25f), -position.x + Random.Range(-0.25f, 0.25f)).normalized;

        float leftDist = CheckFreeDistance(leftDodge);
        float rightDist = CheckFreeDistance(rightDodge);

        Vector2 dodgeDirection = (leftDist == rightDist) ?
            (Random.Range(0, 2) == 0 ? leftDodge : rightDodge) :
            (leftDist > rightDist ? leftDodge : rightDodge);

        knockback.StartKnockback(Random.Range(3 + speed, 4 + speed), dodgeDirection);
    }

    private float CheckFreeDistance(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 3f, LayerMask.GetMask("Walls"));
        return hit.collider == null ? 3f : hit.distance;
    }

    private void Shoot()
    {
        if (target == null) return;

        attackTime = Time.time + attackCooldown;

        if (Random.Range(0, dist) > 5) return;

        Vector2 direction = target.transform.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, LayerMask.GetMask("Walls"));
        if (hit.collider == null)
        {
            seeEnemy = true;
            knockback.StartKnockback(2.5f, transform.position - target.transform.position);
            clip.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (transform.localScale.x == 1 ? 0 : 180));
            ParticleManager.PlayParticle(5, clip.position);
            pool.Get().Fire(0);
        }
        else seeEnemy = false;
    }

    private IEnumerator AttackTimer()
    {
        while (!HealthComponent.IsDead)
        {
            yield return new WaitForSeconds(0.3f + HealthComponent.GetHealthPercent());
            ShockAttack();
        }
    }

    private void ShockAttack()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Creature creature))
            {
                if (creature.IsEnemyTo(this)) creature.HealthComponent.TakeDamage(10);
            }
        }

        ParticleManager.PlayParticle(4, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (HealthComponent.IsDead || HealthComponent.IsWeak) return;
        if (collision.CompareTag("PlayerBullets"))
            Evade(collision.transform.position - transform.position);
    }
    */
}
