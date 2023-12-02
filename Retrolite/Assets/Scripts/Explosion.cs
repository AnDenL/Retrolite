using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Health barrelHealth;
    public LayerMask enemyLayers, explosionLayers;
    public GameObject decal;

    void Start()
    {
        barrelHealth = GetComponent<Health>();
    }
    void BarrelExplosion()
    {
        Collider2D[] hitEmenies = Physics2D.OverlapCircleAll(transform.position, 2f, enemyLayers);
        Instantiate(decal, transform).GetComponent<Transform>().parent = null;
        foreach (Collider2D enemy in hitEmenies) {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, 2f ,explosionLayers);
            if (hitX.collider.tag == "Enemy" || hitX.collider.tag == "Player")
            {
                if (barrelHealth.healthPoint < 0) enemy.GetComponent<Health>().SetHealth(barrelHealth.healthPoint * -1);
                StartCoroutine(Knockback(enemy.GetComponent<Transform>()));
            }
        } 
    }
    IEnumerator Knockback(Transform enemyTransform)
    {
        Vector2 direction = enemyTransform.position - transform.position;
        float timer = 0.5f;
        while(timer > 0)
        {
            enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, transform.position, (8 * timer) * (-Time.deltaTime));
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
