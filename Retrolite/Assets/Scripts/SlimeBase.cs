using UnityEngine;
using System.Collections;

public class SlimeBase : Creature
{
    [Header("Slime")]
    [SerializeField] private float jumpTime;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem hitEffect;

    private bool isAttacking = false;
    private Animator animator;
    private Vector3 targetPosition;
    private float attackTime;

    private void Start()
    {
        HealthComponent.OnDeath += DeathEffect;
        animator = GetComponent<Animator>();
        StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        while (!HealthComponent.IsDead)
        {
            float attackTime = Random.Range(3f, 4f);
            yield return new WaitForSeconds(attackTime);

            if (!isAttacking)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.5f);
                target = FindTarget();
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        if (target != null)
            targetPosition = target.transform.position - transform.position;
        else
            targetPosition = Random.insideUnitCircle * speed;

        float t = jumpTime;

        while (t > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + targetPosition, speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + t);
            t -= Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

    private void DeathEffect()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (attackTime > Time.time) return;
        if (collision.gameObject.TryGetComponent(out Creature creature))
        {
            if (creature.IsEnemyTo(this))
            {
                creature.HealthComponent.TakeDamage(damage);
                attackTime = Time.time + 1f;
            }
        }
    }
}
