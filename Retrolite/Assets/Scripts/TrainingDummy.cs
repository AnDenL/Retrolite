using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : HealthBase
{
    [SerializeField] private TextMeshPro _text;

    private float _dps;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Hit(float damage)
    {
        _dps += damage;
        _text.text = "" + Math.Round(_dps, 1);
        animator.SetTrigger("Hit");
        StartCoroutine(AfterDamage(damage));
    }

    protected override void Heal(float healing)
    {
        _text.text = "<color=green>" + healing;
    }

    private IEnumerator AfterDamage(float damage)
    {
        yield return new WaitForSeconds(1);
        _dps -= damage;
        _text.text = "" + Math.Round(_dps, 1);
    }

    public override float HealthPercent()
    {
        return 1;
    }
}
