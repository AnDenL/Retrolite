using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public float Weigth = 1;
    public float Multiplier = 1;

    private Coroutine coroutine;

    public void StartKnockback(float strength, Vector2 dir)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(DoKnockback(strength, dir));
    }
    private IEnumerator DoKnockback(float strength, Vector2 dir)
    {
        float duration = Mathf.Sqrt(strength) / Weigth;
        float elapsed = 0;
        while (elapsed < duration)
        {
            float factor = 1 - (elapsed / duration);
            transform.position += factor * Multiplier * strength * Time.deltaTime * (Vector3)dir;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Multiplier = 1;
    }
}