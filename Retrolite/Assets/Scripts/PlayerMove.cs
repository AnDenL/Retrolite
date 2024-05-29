using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public float horizontalMove;
    public float verticalMove;
    public AudioClip walkSound;

    private int direction;
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
        animator.SetInteger("Move", Convert.ToInt32(horizontalMove*direction + Mathf.Abs(verticalMove)));
        Vector2 MoveDirection = new Vector2(transform.position.x + horizontalMove,transform.position.y + verticalMove);
        transform.position = Vector2.MoveTowards(transform.position,MoveDirection,speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        Flip();
    }
    void Flip()
    {
        pos = main.WorldToScreenPoint(transform.position);
        if(Input.mousePosition.x < pos.x){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            direction = -2;
        }
        else {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            direction = 2;
        }
    }
    void FlipGamepad()
    {
        if(Input.GetAxis("JoyX") < 0){
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            direction = -2;
        }
        else {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            direction = 2;
        }
    }
    void PlayWalkSound() 
    {   
        sound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        sound.PlayOneShot(walkSound);
    }
}
