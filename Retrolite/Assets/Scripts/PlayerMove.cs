using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public float horizontalMove;
    public float verticalMove;
    public AudioClip walkSound;
    private AudioSource sound;
    private Animator animator;
    private Vector3 pos;
    private Camera main;
    private void Start()
    {
        sound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        animator.SetInteger("Move", Convert.ToInt32(horizontalMove*2 + Mathf.Abs(verticalMove)));
        pos = main.WorldToScreenPoint(transform.position);
        Vector2 MoveDirection = new Vector2(transform.position.x + horizontalMove,transform.position.y + verticalMove);
        transform.position = Vector2.MoveTowards(transform.position,MoveDirection,speed * Time.deltaTime);
        Flip();
    }
    void Flip()
    {
        if(Input.mousePosition.x < pos.x){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if(Input.mousePosition.x > pos.x){
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    void PlayWalkSound() 
    {   
        sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        sound.PlayOneShot(walkSound);
    }
}
