using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForestSpirit : MonoBehaviour
{
    public List<GameObject> objects;
    public GameObject Target = null;
    public float speed;

    private float findTargetCooldown;
    private float rDirectionCooldown;
    private float speedModifier = 1f;
    private GameObject player;
    private Vector2 MoveDirection;
    private Vector2 randomDirection;
    
    private void Start()
    {
        player = Game.Player;
        findTargetCooldown = Time.time + Random.Range(4f,6f);
    }

    private void Update()
    {
        if (Target == null) 
        {
            MoveDirection = transform.position + player.transform.position;
            speedModifier = 0.75f;
            if(findTargetCooldown < Time.time) FindTarget();
        }
        else 
        {
            MoveDirection = Target.transform.position;
            speedModifier = 1f;
        }

        transform.position = Vector2.MoveTowards(transform.position,MoveDirection,(speed * speedModifier) * Time.deltaTime);

        if(rDirectionCooldown < Time.time)
        {
            randomDirection = new Vector2((5 * Random.Range(-3, 4)) - transform.position.x, (5 * Random.Range(-3, 4)) - transform.position.y);
            rDirectionCooldown = Time.time + Random.Range(0.1f,0.3f);
        }
        transform.position = Vector2.MoveTowards(transform.position, randomDirection, 1.5f * Time.deltaTime);

        if (Target == null && findTargetCooldown < Time.time && Vector2.Distance(transform.position,player.transform.position) > 20)
        {
            gameObject.GetComponent<Health>().SetHealth(5);
        }
    }

    private void FindTarget()
    {
        if(objects.Count != 0) Target = objects[Random.Range(0,objects.Count)];
    }

    private void Attack()
    {
        PlayerMove playermovement = player.GetComponent<PlayerMove>();
        Vector2 prediction = new Vector2(playermovement.horizontalMove * (Vector2.Distance(transform.position,player.transform.position) / 3),playermovement.verticalMove * (Vector2.Distance(transform.position,player.transform.position) / 3));
        Vector3 ThrowDirection = new Vector3(player.transform.position.x + prediction.x,player.transform.position.y + prediction.y,0);
        Target.GetComponent<ThrowObject>().Throw(ThrowDirection);
        findTargetCooldown = Time.time + Random.Range(4f,6f);
        Target = null;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == Target)
        {
            objects.Remove(Target);
            Invoke("Attack",1f);
        }
    }
}