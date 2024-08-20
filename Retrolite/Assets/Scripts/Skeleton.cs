using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public AudioClip AttackSound;
    public float speed;
    public LayerMask raycastLayer, wallLayer;
    public Transform sprite;
    private bool seePlayer,chase = false;
    private Vector3 pos, Offset;
    private Transform Player;
    private AudioSource sound;
    private Animator animator;
    private void Start()
    {
        Player = Game.Player.transform;
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        StartCoroutine(CheckTimer());
    }
    private IEnumerator CheckTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            CheckPlayer();
        }
    }
    private IEnumerator RunToLastPlayerPos()
    {
        chase = false;
        Vector3 Lastpos = Player.position;
        float t = 4;
        while (t > 1)
        {
            Vector3 p = transform.position;
            t -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, Lastpos + Offset, speed * Time.deltaTime);
            if(Vector2.Distance(transform.position, Lastpos) < 2f || seePlayer) break;
            yield return null;
        }
        animator.SetBool("Move", false);
    }
    private void Update()
    {
        if (pos.x > 0) sprite.localScale = new Vector2(1, 1);
        else sprite.localScale = new Vector2(-1, 1);
        if (seePlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + pos, speed * Time.deltaTime);
            animator.SetBool("Move", true);
        }
        else
        {
            if(chase) StartCoroutine(RunToLastPlayerPos());
        }
    }
    private void CheckPlayer()
    {
        float side = sprite.localScale.x;
        Vector3 direction = Player.position - transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, 12f, raycastLayer);

        if(rayHit.collider != null)
        {
            if (rayHit.collider.tag == "Player")
            {
                pos = new Vector2(direction.x * Offset.x, direction.y * Offset.y);
                seePlayer = true;
                if (!seePlayer) sound.PlayOneShot(AttackSound);
            }
            else
            {
                if (seePlayer) chase = true;
                seePlayer = false;
            }
        }
        else
        {
            if (seePlayer) chase = true;
            seePlayer = false;
        }

        rayHit = Physics2D.Raycast(transform.position, new Vector2(1 * side, 0), 1f, wallLayer);

        if (rayHit.collider != null)
        {
            Offset.x = 0.1f;
        }
        else Offset.x = 1;

        rayHit = Physics2D.Raycast(transform.position, new Vector2(0, pos.y), 1f, wallLayer);

        if (rayHit.collider != null)
        {
            Offset.y = 0.1f;
        }
        else Offset.y = 1;
    }
}
