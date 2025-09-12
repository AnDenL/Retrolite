using UnityEngine;

public class Robot : Creature
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float attackCooldown = 1;
    [SerializeField] private float speed = 3;
    [SerializeField] private LayerMask rayMask;
    [SerializeField] private float curve;

    private Vector2 targetPos;
    private float attackTime;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private Vector2 GetPath()
    {
        target = FindTarget();
        targetPos = Random.insideUnitCircle;

        Vector2 direction = transform.position - target.transform.position;

        var ray = Physics2D.Raycast(transform.position, transform.position - target.transform.position, 8f, rayMask);

        if (ray.collider == null)
        {
            curve = 0;
            return target.transform.position;
        }

        curve += Time.time;


        return targetPos; 
    }

    private void Shoot()
    {
        if (attackTime < Time.time) return;
        Instantiate(bullet, transform.position, Quaternion.identity);
        attackTime = Time.time + attackCooldown;
    }
}