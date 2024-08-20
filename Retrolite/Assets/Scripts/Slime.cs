using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public AudioClip slimeSound;
    public float minAttackTime = 2f;
    public float maxAttackTime = 4f;
    public GameObject miniSlimes;
    public bool spawnBullets;
    public bool IsDead;
    
    private Transform playerTransform;
    private Animator animator;
    private AudioSource sound;
    private bool isAttacking = false;

    private void Awake()
    {
        playerTransform = Game.Player.transform;
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }
    private void Start()
    {
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
        if (spawnBullets) gameObject.GetComponent<Murshroom>().SporeExplosion();
        float moveSpeed = 6;
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

        sound.pitch = Random.Range(0.8f, 1.2f);
        sound.PlayOneShot(slimeSound);

        while (moveSpeed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (targetPosition * 2), moveSpeed * Time.deltaTime);
            moveSpeed -= Time.deltaTime * 5;
            yield return null;
        }

        isAttacking = false;
    }
    private void Divide()
    {
        miniSlimes.SetActive(true);
        IsDead = true;
    }
}