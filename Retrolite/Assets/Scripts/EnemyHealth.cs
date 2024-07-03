using TMPro;
using System;
using UnityEngine;

public class EnemyHealth : HealthBase
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _destroyDelay;
    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    protected override void Hit(float damage)
    {
        base.Hit(damage);
        _text.text = "" + Math.Round(_health, 2);
        animator.SetTrigger("Hit");
    }
    protected override void Heal(float healing)
    {
        base.Heal(healing);
        _text.text = "" + Math.Round(_health, 2);
    }
    protected override void Death()
    {
        base.Death();
        Destroy(gameObject, _destroyDelay);
        Game.kills++;
        animator.SetTrigger("Death");
    }
}
