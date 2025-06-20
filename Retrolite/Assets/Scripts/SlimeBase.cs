using UnityEngine;
using System.Collections;

public class SlimeBase : HealthBase
{
    [SerializeField]
    protected TextMesh healthLabel;

    [Header("Slime")]
    [SerializeField]
    protected float jumpTime;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float damage;

    protected bool isAttacking = false;
    protected bool isDead = false;
    protected Animator animator;
    protected Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        StartCoroutine(AttackTimer());
    }

    private IEnumerator AttackTimer()
    {
        while (!isDead)
        {
            float attackTime = Random.Range(3f, 4f);
            yield return new WaitForSeconds(attackTime);

            if (!isAttacking)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(Attack());
            }
        }
    }

    protected IEnumerator Attack()
    {
        isAttacking = true;
        if (Vector2.Distance(transform.position, Player.instance.transform.position) < 8f)
            targetPosition = Player.instance.transform.position - transform.position;
        else
            targetPosition = Random.insideUnitCircle * speed;

        float t = jumpTime;

        while (t > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + targetPosition, speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.y + t);
            t -= Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthLabel.text = health.ToString();
    }

    protected override void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            collision.gameObject.GetComponent<Player>()?.TakeDamage(damage);
        }
    }
}
