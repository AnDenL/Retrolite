using System.Collections;
using UnityEngine;

public class GolemBody : MonoBehaviour
{
    public Transform head;
    public GameObject Text;
    public int charges = 0;
    public GameObject Head;
    public GameObject ShieldEffect, bullet;
    public SpriteRenderer sphere_1, sphere_2,sphere_3;
    public AudioClip step, DashSound;

    private PlayerMove PlayerMovement;
    private Health hp;
    private AudioSource sound;
    private float ChargeTime;
    private float speed;
    private float dashTime;
    private bool FacingRight = false;
    private bool isWalk = false;
    private Transform player;
    private Animator animator;

    private void Start()
    {
        hp = Head.GetComponent<Health>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.transform;
        PlayerMovement = p.GetComponent<PlayerMove>();
        sound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        Invoke("StartWalk", 2f);
        dashTime = Time.time + Random.Range(6f,15f);
    }

    private void Update()
    {
        if(isWalk) {
            speed = 5.5f - Mathf.Sqrt(Vector2.Distance(head.position,transform.position)); 
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
            animator.SetBool("IsWalk", true);
            animator.speed = speed / 5f;
        }
        else {
            animator.SetBool("IsWalk", false);
            animator.speed = 1;
        }
        if(Vector2.Distance(transform.position, player.position) < 4f && dashTime < Time.time)
        {   
            StartCoroutine(Dash());
            dashTime = Time.time + Random.Range(6f,12f);
        }
        if(ChargeTime < Time.time)
        {
            if(charges < 3)charges ++;
            else hp.Heal(10);

            if(charges > 0)sphere_1.color = new Color(1,1,1,1);
            else sphere_1.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 1)sphere_2.color = new Color(1,1,1,1);
            else sphere_2.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 2)sphere_3.color = new Color(1,1,1,1);
            else sphere_3.color = new Color(0.1f,0.1f,0.1f,1);

            ChargeTime = Time.time + 5;
        }
        
        if ((transform.position.x - player.transform.position.x) < 0 && FacingRight)
        {
            Flip();
        }
        else if ((transform.position.x - player.transform.position.x) > 0 && !FacingRight)
        {
            Flip();
        }
    }
    private void StepSound()
    {
        sound.pitch = Random.Range(0.8f,1.2f);
        sound.PlayOneShot(step);
    }

    public void Shield()
    {
        charges--;

        if(charges > 0)sphere_1.color = new Color(1,1,1,1);
        else sphere_1.color = new Color(0.1f,0.1f,0.1f,1);
        if(charges > 1)sphere_2.color = new Color(1,1,1,1);
        else sphere_2.color = new Color(0.1f,0.1f,0.1f,1);
        if(charges > 2)sphere_3.color = new Color(1,1,1,1);
        else sphere_3.color = new Color(0.1f,0.1f,0.1f,1);

        GameObject Effect = Instantiate(ShieldEffect, transform.GetChild(0));
        Effect.transform.position = transform.position;
        Instantiate(bullet, transform.GetChild(0));
    }

    private void Death()
    {
        Head.GetComponent<GolemHead>().charges = 0;
        hp.SetHealth(hp.healthPoint);
    }
    private IEnumerator Dash()
    {
        float moveSpeed = 3;
        isWalk = false;

        Vector3 targetPosition;
        Vector3 movePrediction = new Vector3(PlayerMovement.horizontalMove, PlayerMovement.verticalMove, 0);
        targetPosition = (player.position + (movePrediction*2)) - transform.position;

        sound.pitch = Random.Range(0.8f, 1.2f);
        sound.PlayOneShot(DashSound);

        while (moveSpeed > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (targetPosition * 2), (speed * moveSpeed) * Time.deltaTime);
            moveSpeed -= Time.deltaTime * 4;
            if(moveSpeed < 0.5f) isWalk = true;
            yield return null;
        }
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;
        transform.localScale = LocalScale;
        Text.transform.localScale = LocalScale / 20;
    }

    private void StartWalk() => isWalk = true;
}
