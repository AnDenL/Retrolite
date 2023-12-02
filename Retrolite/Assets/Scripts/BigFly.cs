using System.Collections;
using UnityEngine;

public class BigFly : MonoBehaviour
{
    public int damage = 1;
    public float minAttackTime = 2f;
    public float maxAttackTime = 4f;
    public GameObject fly;
    public float moveSpeed;

    private bool IsDead;
    private Transform playerTransform;
    private Animator animator;
    private bool isAttacking = false;
    private Vector2 moveDirection;
    private AudioSource sound;
    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        sound.pitch = Random.Range(0.5f,0.6f);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        StartCoroutine(AttackTimer());
    }
    private void FixedUpdate()   
    {
        moveDirection = new Vector2((5 * Random.Range(-3, 4)) - transform.position.x, (5 * Random.Range(-3, 4)) - transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, moveDirection,2 * Time.deltaTime);
    }
    private IEnumerator AttackTimer()
    {
        while (!IsDead)
        {
            float attackTime = Random.Range(minAttackTime, maxAttackTime);
            yield return new WaitForSeconds(attackTime);
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }
    private IEnumerator Attack()
    {
        moveSpeed = 1f;
        isAttacking = true;

        Vector3 targetPosition;
        if (Vector2.Distance(transform.position, playerTransform.position) < 8f)
        {
            targetPosition = playerTransform.position - transform.position;
        }
        else
        {
            targetPosition = transform.position + new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f));
        }

        while (moveSpeed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (targetPosition * 2), moveSpeed * Time.deltaTime * 9f);
            moveSpeed -= Time.deltaTime * 2;
            yield return null;
        }

        isAttacking = false;
    }
    private void SpawnMiniFlyes()
    {
        GameObject flyes = Instantiate(fly, transform.parent);
        flyes.transform.position = new Vector2(transform.position.x, transform.position.y);
        flyes.transform.parent = null;
    }
}
