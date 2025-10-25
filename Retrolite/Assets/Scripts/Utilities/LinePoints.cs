using UnityEngine;

public class LinePoints : MonoBehaviour
{
    public Line[] lines;

    private void LateUpdate()
    {
        foreach (var line in lines)
        {
            line.UpdateLine();
        }
    }

    public void SetEndPointPositions(Transform position)
    {
        foreach (var line in lines)
            line.pointTransform[^1] = position;
    }

    public void SetEndPointPositions(Transform[] position)
    {
        for (int i = 0; i < lines.Length; i++)
            lines[i].pointTransform[^1] = position[i];
    }
    
    public Transform[] GetEndPointPositions()
    {
        Transform[] endPoints = new Transform[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            endPoints[i] = lines[i].pointTransform[^1];
        }
        return endPoints;
    }
}

[System.Serializable]
public struct Line
{
    public LineRenderer lineRenderer;
    public Transform[] pointTransform;

    public readonly void UpdateLine()
    {
        for (int i = 0; i < pointTransform.Length; i++)
        {
            lineRenderer.SetPosition(i, pointTransform[i].position);
        }
    }
}
