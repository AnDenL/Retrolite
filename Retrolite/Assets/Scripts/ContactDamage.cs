using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public float damage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player health = collision.gameObject.GetComponent<Player>();
            health.SetHealth(damage);
        }
    }
}
