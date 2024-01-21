using UnityEngine;

public class GolemBody : MonoBehaviour
{
    public Transform head;
    public GameObject Text;
    public int charges = 0;
    public GameObject Head;
    public GameObject ShieldEffect, bullet;
    public SpriteRenderer sphere_1, sphere_2,sphere_3;
    public AudioClip step;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        Invoke("StartWalk", 2f);
    }

    private void Update()
    {
        if(isWalk) {
            speed = 5f - Mathf.Sqrt(Vector2.Distance(head.position,transform.position)); 
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
            animator.SetBool("IsWalk", true);
            animator.speed = speed / 5f;
        }
        else {
            speed = 0;
            animator.SetBool("IsWalk", false);
            animator.speed = 1;
        }
        if(ChargeTime < Time.time)
        {
            if(charges < 3)charges ++;
            else hp.Heal(2);

            if(charges > 0)sphere_1.color = new Color(1,1,1,1);
            else sphere_1.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 1)sphere_2.color = new Color(1,1,1,1);
            else sphere_2.color = new Color(0.1f,0.1f,0.1f,1);
            if(charges > 2)sphere_3.color = new Color(1,1,1,1);
            else sphere_3.color = new Color(0.1f,0.1f,0.1f,1);

            ChargeTime = Time.time + 4;
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
