using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ArcAnim : MonoBehaviour
{
    [SerializeField] float maxHeight = 1f;
    public float duration = 0.6f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void DropTo(Vector3 targetPosition, System.Action onFinish = null)
    {
        StartCoroutine(DropCoroutine(targetPosition, onFinish));
    }

    private IEnumerator DropCoroutine(Vector3 targetPos, System.Action onFinish)
    {
        Vector3 startPos = transform.position;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.excludeLayers = LayerMask.GetMask("Obstacles");

        for (float t = 0; t < 1f; t += Time.deltaTime / duration)
        {
            float h = (1f - Mathf.Pow(2f * t - 1f, 2f)) * maxHeight;
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            sr.transform.localPosition = new Vector2(0, h);

            yield return null;
        }

        if (col) col.includeLayers = LayerMask.GetMask("Obstacles");

        onFinish?.Invoke();
    }
}
