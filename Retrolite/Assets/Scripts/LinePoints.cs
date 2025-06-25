using UnityEngine;

public class LinePoints : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Transform[] points;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
