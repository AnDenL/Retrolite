using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ForestGenerator", menuName = "Generators/Forest", order = -250)]
public class ForestGenerator : MapGenerator
{
    [Header("Generation Options")]
    public Vector2Int Size;
    public float MinDistance, MaxDistance;
    public float MinAreaSize, MaxAreaSize;
    public float RandomAngle;
    public float JitterAmount = 1.5f;
    public float NoiseStrength, scale;

    [Header("Branching")]
    public int BranchCount = 4;
    public int MinPointsPerBranch, MaxPointsPerBranch;

    private List<KeyPoint> keyPoints;
    private float[,] map;

    public override float[,] Generate()
    {
        map = new float[Size.x, Size.y];
        keyPoints = new List<KeyPoint>();

        Vector2 center = new Vector2(Size.x / 2f, Size.y / 2f);
        KeyPoint startPoint = new KeyPoint();
        startPoint.Position = Vector2Int.RoundToInt(center);
        startPoint.AreaSize = MaxAreaSize;
        keyPoints.Add(startPoint);

        for (int b = 0; b < BranchCount; b++)
        {
            float angle = (360f / BranchCount) * b + Random.Range(-RandomAngle, RandomAngle);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector2 currentPos = center;
            int PointsPerBranch = Random.Range(MinPointsPerBranch, MaxPointsPerBranch);

            for (int p = 0; p < PointsPerBranch; p++)
            {
                float value = Random.value;
                float distance = Mathf.Lerp(MinDistance, MaxDistance, value);
                Vector2 jitter = Random.insideUnitCircle * JitterAmount;

                currentPos += direction * distance + jitter;
                Vector2Int posInt = Vector2Int.RoundToInt(currentPos);

                if (posInt.x < 0 || posInt.y < 0 || posInt.x >= Size.x || posInt.y >= Size.y)
                    continue;

                KeyPoint point = new KeyPoint
                {
                    Position = posInt,
                    AreaSize = Mathf.Lerp(MinAreaSize, MaxAreaSize, value),
                    Angle = angle + Random.Range(-RandomAngle, RandomAngle)
                };

                keyPoints.Add(point);
            }
        }

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                float value = 1;
                
                foreach (KeyPoint point in keyPoints)
                {
                    float dist = Vector2Int.Distance(new Vector2Int(x, y), point.Position);
                    value -= Mathf.Clamp((point.AreaSize - dist) / point.AreaSize, 0, 1);
                }

                map[x, y] = value;
            }
        } 

        AddNoise();

        return map;
    }

    private void AddNoise()
    {
        float offset = Random.Range(-10,10);
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                map[x, y] += Mathf.PerlinNoise((x / scale) + offset, (y / scale) + offset) * NoiseStrength;
            }
        }
    }
}

public struct KeyPoint
{
    public float AreaSize;
    public float Angle;
    public Vector2Int Position;
}
