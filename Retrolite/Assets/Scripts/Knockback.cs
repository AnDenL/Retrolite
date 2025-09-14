using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public float Weigth = 1;
    public bool inMoveNow;

    private Coroutine coroutine;

    public void StartKnockback(float strength, Vector2 dir)
    {
        dir.Normalize();
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(DoKnockback(strength, dir));
    }
    private IEnumerator DoKnockback(float strength, Vector2 dir)
    {
        inMoveNow = true;
        float duration = Mathf.Sqrt(strength) / Weigth;
        float elapsed = 0;
        while (elapsed < duration)
        {
            float factor = 1 - (elapsed / duration);
            transform.position += factor * strength * Time.deltaTime * (Vector3)dir;
            elapsed += Time.deltaTime;
            yield return null;
        }
        inMoveNow = false;
    }
}