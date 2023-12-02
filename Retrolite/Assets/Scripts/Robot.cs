using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float speed;
    public GameObject Text;
    private float EnemyDirection = 0f;
    private bool FacingRight = true;
    private float attackTime;
    private Vector2 targetPosition;
    private Animator animator;
    private GameObject player;
    private bool isAttacking = false;
    private void Awake() 
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
    }
    private void Start() 
    {
        StartCoroutine(AttackTimer());
    }
    private void Update() 
    {
        targetPosition = player.transform.position;
        EnemyDirection = transform.position.x - player.transform.position.x;
        if (EnemyDirection < 0 && FacingRight)
        {
            Flip();
        }
        else if (EnemyDirection > 0 && !FacingRight)
        {
            Flip();
        }
    }
    private IEnumerator AttackTimer()
    {
        while (true)
        {
            float attackTime = 3;
            yield return new WaitForSeconds(attackTime);
            if (!isAttacking)
            {
                animator.SetTrigger("Shoot");
            }
        }
    }
    private void FixedUpdate() 
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 5f) 
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, -speed / 5);
        else if (Vector2.Distance(transform.position, player.transform.position) > 8f) 
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed / 5);
    }
    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;
        transform.localScale = LocalScale;
        Text.transform.localScale = LocalScale;
    }
    private void Shoot()
    {
        Instantiate(BulletPrefab,transform);
    }
}
