using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public float damage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && damage != 0)
        {
            Health health = collision.gameObject.GetComponent<Health>();
            health.SetHealth(damage);
        }
    }
}
