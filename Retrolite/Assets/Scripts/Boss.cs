using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform[] targets;
    public float minAttackTime = 2f;
    public float maxAttackTime = 4f;
    public Transform target;
    public Vector2 moveTarget;
    public GameObject bulletPrefab,bossItem; 
    public bool IsDead;
    public GameObject[] items;

    private Transform playerTransform;
    private int nextAttack, itemNumber, goodItem;
    private Animator animator;
    private Bounce bounce;
    private BossItems bossItems;
    private GameObject bullet;
    private bool IsMove = false;
    private bool isAttacking = false;
    private void Start()
    {
        moveTarget = new Vector2(transform.position.x,transform.position.y);
        nextAttack = Random.Range(0,2);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        StartCoroutine(AttackTimer());
    }
    private IEnumerator AttackTimer()
    {
        while (!IsDead)
        {
            float attackTime = Random.Range(minAttackTime, maxAttackTime);
            yield return new WaitForSeconds(attackTime);
            if (nextAttack > 4)
            {
                SpecialAttack();
                nextAttack = Random.Range(0,2);
            }
            else if (!isAttacking)
            {
                Attack();
            }
        }
    }
    private void Attack()
    {
        nextAttack++;
        target.localRotation = Quaternion.Euler(0, 0, Random.Range(-30,30));

        for (int i = 0;i < targets.Length;i++)
        {
            bullet = Instantiate(bulletPrefab, transform.parent);
            bullet.transform.position = transform.position;
            bounce = bullet.GetComponent<Bounce>();
            bounce.directionX = transform.position.x - targets[i].position.x;
            bounce.directionY = transform.position.y - targets[i].position.y;
        }
    }
    private void SpecialAttack()
    {
        goodItem = Random.Range(0,5);
        IsMove = true;
        animator.SetBool("Attack",true);
        itemNumber = 0;
    }
    private void SpawnItem()
    {
        items[itemNumber] = Instantiate(bossItem, transform.parent);
        bossItems = items[itemNumber].GetComponent<BossItems>();
        bossItems.pos = new Vector2(playerTransform.position.x, playerTransform.position.y);
        bossItems.animator = animator;
        bossItems.boss = GetComponent<Boss>();
        if(itemNumber == goodItem)bossItems.GetComponent<Health>().healthPoint = 5;
        itemNumber++;
    }
    private void Update()
    {
        if (IsMove)
        {
            transform.position = Vector2.MoveTowards(transform.position,moveTarget,(2 * Vector2.Distance(transform.position,moveTarget)) * Time.deltaTime);
            if (Vector2.Distance(transform.position,moveTarget) < 0.1f) IsMove = false;
        }
    }
    public void Delete()
    {
        for(int i = 0;i < items.Length; i++)
        {
            Destroy(items[i]);
        }
    }
}