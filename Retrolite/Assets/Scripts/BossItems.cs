using UnityEngine;

public class BossItems : MonoBehaviour
{
    public GameObject Button;
    public Animator animator;
    public Vector2 pos;
    public Boss boss;
    public GameObject barrel;
    public Sprite[] image;
    private bool IsMove = true, explosion;
    private SpriteRenderer sprite;
    private void Start() 
    {
        sprite = barrel.GetComponent<SpriteRenderer>();
        if(explosion) sprite.sprite = image[0];
        else sprite.sprite = image[1];
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position,pos) > 0.1f) 
            transform.position = Vector2.MoveTowards(transform.position,pos,(Vector2.Distance(transform.position,pos) * 3) * Time.deltaTime);
        else if(IsMove)
        {
            IsMove = false;
            GetComponent<ContactDamage>().damage = 0;
            GetComponent<Collider2D>().isTrigger = true;
            Invoke("Activate",0.2f);
        }
    }
    private void Activate()
    {
        barrel.GetComponent<Health>().SetHealth(0);
        animator.SetBool("Attack",false);
    }
}
