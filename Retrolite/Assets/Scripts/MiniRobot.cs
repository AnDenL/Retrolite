using System.Collections;
using UnityEngine;

public class MiniRobot : MonoBehaviour
{
    public int damage = 1;
    public float minAttackTime = 2f;
    public float maxAttackTime = 4f;
    public bool IsDead;
    public GameObject text;
    
    private Transform playerTransform;
    private Animator animator;
    private bool isAttacking = false;
    private void Start()
    {
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
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }
    private IEnumerator Attack()
    {
        float moveTime = 0;
        isAttacking = true;

        animator.SetBool("Move",true);
        Vector3 targetPosition;
        if (Vector2.Distance(transform.position, playerTransform.position) < 8f)
        {
            moveTime = Random.Range(0.5f,0.8f);
            targetPosition = playerTransform.position - transform.position;
        }
        else
        {
            moveTime = Random.Range(0.3f,0.5f);
            targetPosition = transform.position + new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f));
        }
        if(targetPosition.x < 0){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            text.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            text.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        while (moveTime > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (targetPosition * 2),6 * Time.deltaTime);
            moveTime -= Time.deltaTime;
            yield return null;
        }
        animator.SetBool("Move",false);
        isAttacking = false;
    }
}
