using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcAnim : MonoBehaviour
{
    [SerializeField] float maxHeight = 1f;
    public float duration = 0.6f;

    public Transform sr;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (!sr) sr = transform.Find("Sprite");
        rb = GetComponent<Rigidbody2D>();
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
            
            rb.velocity = (targetPos - startPos) / duration;
            sr.localPosition = new Vector2(0, h);

            yield return null;
        }

        if (col) 
        {
            if (!col.enabled) col.enabled = true;
            col.includeLayers = LayerMask.GetMask("Obstacles");
        }

        onFinish?.Invoke();
    }
}
