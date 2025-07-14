using System.Collections;
using UnityEngine;

public class ArcAnim : MonoBehaviour
{
    [Header("Curve")]
    [SerializeField] AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float maxHeight = 0.75f;
    [SerializeField] float duration = 0.6f;

    [Header("Shadow")]
    [SerializeField] Transform shadow;
    [SerializeField] Vector3 shadowOffset = new Vector3(0, -0.25f, 0);

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (shadow == null && transform.childCount > 0)
            shadow = transform.GetChild(0);
    }

    public void DropTo(Vector3 targetPosition, System.Action onFinish = null)
    {
        StartCoroutine(DropCo(targetPosition, onFinish));
    }

    IEnumerator DropCo(Vector3 targetPos, System.Action onFinish)
    {
        Vector3 startPos = transform.position;

        if (sr) sr.sortingOrder = -2;
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        for (float t = 0; t < 1f; t += Time.deltaTime / duration)
        {
            float h = heightCurve.Evaluate(t) * maxHeight;
            transform.position = Vector3.Lerp(startPos, targetPos, t) + Vector3.up * h;

            if (shadow)
                shadow.localPosition = shadowOffset - Vector3.up * h;

            yield return null;
        }

        transform.position = targetPos;
        if (shadow) shadow.localPosition = shadowOffset;

        if (sr) sr.sortingOrder = -3;
        if (col) col.enabled = true;

        onFinish?.Invoke();
    }
}
