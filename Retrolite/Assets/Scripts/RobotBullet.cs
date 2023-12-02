using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBullet : MonoBehaviour
{
    public int damage = 3;
    public float bulletSpeed;
    public LayerMask Layers, explosionLayers;
    private Rigidbody2D Rigidbody;
    private Animator animator;
    private GameObject player;
    private Vector3 direction;
    private float rotationZ;
    private void Start()
    {
        transform.parent = null;
        Rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        var newPosition = player.transform.position;
        direction = newPosition - transform.position;
        rotationZ = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, rotationZ);
        Rigidbody.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
    }
    private void FixedUpdate() 
    {
        var newPosition = player.transform.position;
        direction = newPosition - transform.position;
        rotationZ = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, rotationZ);
        Rigidbody.AddForce(transform.right * 0.2f, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Enemy" && collision.tag != "Empty") animator.SetTrigger("Explosion");
    }
    private void Explosion()
    {
        Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(transform.position, 2f, Layers);
        foreach (Collider2D enemy in hitEmenies)
        {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, enemy.transform.position, 2f, explosionLayers);
            if (hitX.collider.tag == "Player")
            {
                enemy.GetComponent<Health>().SetHealth(damage);
            }
        }
    }
    private void Destroy() => Destroy(gameObject);
}
