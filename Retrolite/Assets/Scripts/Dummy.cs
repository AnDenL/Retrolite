using UnityEngine;
using System;
using System.Collections;

public class Dummy : HealthBase
{
    [SerializeField] private TextMesh text;

    private Animator animator;
    private float t;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void TakeDamage(float damage)
    {
        text.gameObject.SetActive(true);
        text.text = Math.Round(damage, 2).ToString();

        if (Player.instance.transform.position.x > transform.position.x) animator.SetTrigger("Hit");
        else animator.SetTrigger("HitBack");

        StartCoroutine(TextFade());
    }

    public override float GetHealthPercent() => 1;

    private IEnumerator TextFade()
    {
        if (t > 0)
        {
            t = 5f;
            yield return null;
        }
        t = 5f;

        while (t > 0)
        {
            t -= Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, t / 5f);
            yield return null;
        }

        text.gameObject.SetActive(false);
    }
}
