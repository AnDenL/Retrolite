using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField]
    private float damageAmount = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<HealthBase>()?.TakeDamage(damageAmount);
    }
}
