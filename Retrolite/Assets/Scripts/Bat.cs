using System.Collections;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public AudioClip batSound;
    public float speed = 1;    private Health hp;
    private bool isSleeping = true;
    private GameObject player;
    private Animator animator;
    private AudioSource sound;
    private Vector2 moveDirection;

    private const float sleepCheckInterval = 0.5f;

    private void Awake()
    {
        player = Game.Player;
        hp = GetComponent<Health>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();  
    }

    private void Start()
    {
        StartCoroutine(Sleep());
    }

    private void FixedUpdate()   
    {
        if (!isSleeping)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed);

            moveDirection = new Vector2((5 * Random.Range(-3, 4)) - transform.position.x, (5 * Random.Range(-3, 4)) - transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, moveDirection, 0.01f);
        }
    }

    private IEnumerator Sleep()
    {
        while (isSleeping)
        {
            if (hp.healthPoint != hp.maxHealthPoint)
            {
                isSleeping = false;
            }
            else if (Mathf.Abs(player.transform.position.x - transform.position.x) + Mathf.Abs(player.transform.position.y - transform.position.y) < 4)
            {
                isSleeping = false;
            }
            yield return null;
        }

        sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        sound.PlayOneShot(batSound);
        animator.SetTrigger("Awake");
    }
}