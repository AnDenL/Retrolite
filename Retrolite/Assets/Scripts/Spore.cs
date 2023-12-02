using System.Collections;
using UnityEngine;

public class Spore : MonoBehaviour
{
    public Vector2 target;
    public float speed;
    
    private bool isScaling = true;
    
    private void Start()
    {
        float deathTime = Random.Range(5f, 8f);
        StartCoroutine(Scale(deathTime - 1f));
        Destroy(gameObject, deathTime);
    }
    
    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed / 10);
        if (speed > 0.06f) {
            speed -= 0.008f;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            Health health = collision.GetComponent<Health>();
            if (health != null) {
                health.SetHealth(2);
            }
            isScaling = false;
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Empty") && !collision.CompareTag("Enemy")) {
            isScaling = false; 
            Destroy(gameObject);
        }
    }
    
    private IEnumerator Scale(float delay)
    {
        yield return new WaitForSeconds(delay);
        float scale = transform.localScale.x;
        while (isScaling && scale > 0) {
            transform.localScale = new Vector2(scale, scale);
            scale -= transform.localScale.x * Time.deltaTime;
            yield return null;
        }
    }
}