using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public float Weigth = 1;
    public bool inMoveNow => knockbackDuration > Time.time;

    private float knockbackDuration;
    private Coroutine coroutine;
    private Rigidbody2D rb;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void StartKnockback(float strength, Vector2 dir)
    {
        dir.Normalize();
        rb.velocity = Vector2.zero;
        rb.AddForce(strength * dir, ForceMode2D.Impulse);
        knockbackDuration = Mathf.Sqrt(strength) / Weigth;
    }
}
