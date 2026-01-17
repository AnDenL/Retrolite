using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CalculatingSystem;

[CreateAssetMenu(fileName = "ImprovedForestGenerator", menuName = "Game/Generators/ForestImproved", order = -250)]
public class ForestGenerator : MapGenerator
{
    [Header("Generation Options")]
    public float MinDistance, MaxDistance;
    public float MinAreaSize, MaxAreaSize;
    public float RandomAngle;
    public float JitterAmount = 1.5f;
    public float NoiseStrength, scale;

    [Header("Branching")]
    public int BranchCount = 4;
    public int MinPointsPerBranch, MaxPointsPerBranch;

    private HashSet<Vector2Int> enemyPositions; 

    public override void Generate(GenerationContext context)
    {
        for (int x = 0; x < context.Size.x; x++)
            for (int y = 0; y < context.Size.y; y++)
                context.Map[x, y] = 0f;

        var keyPoints = new List<KeyPoint>();
        enemyPositions = new HashSet<Vector2Int>();
        
        GenerateBranches(context.Center, keyPoints, context);

        foreach (var point in keyPoints)
        {
            DrawBrush(context, point);
        }

        AddNoise(context);

        var furthestPoint = keyPoints
            .OrderByDescending(p => Vector2.Distance(p.Position, context.Center))
            .First();
            
        context.EndPoint = furthestPoint.Position;
        context.Enemies = enemyPositions.ToList();
    }

    private void GenerateBranches(Vector2 startPos, List<KeyPoint> points, GenerationContext context)
    {
        points.Add(new KeyPoint { Position = Vector2Int.RoundToInt(startPos), AreaSize = MaxAreaSize });

        for (int b = 0; b < BranchCount; b++)
        {
            Vector2 currentPos = startPos;
            float currentAngle = (360f / BranchCount) * b;

            int steps = Random.Range(MinPointsPerBranch, MaxPointsPerBranch);

            for (int i = 0; i < steps; i++)
            {
                currentAngle += Random.Range(-RandomAngle, RandomAngle);
                
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                float dist = Mathf.Lerp(MinDistance, MaxDistance, Random.value);
                
                currentPos += direction * dist + (Random.insideUnitCircle * JitterAmount);
                Vector2Int posInt = Vector2Int.RoundToInt(currentPos);

                if (posInt.x < 5 || posInt.y < 5 || posInt.x >= context.Size.x - 5 || posInt.y >= context.Size.y - 5)
                    break;

                float size = Mathf.Lerp(MaxAreaSize, MinAreaSize, (float)i / steps); 

                points.Add(new KeyPoint { Position = posInt, AreaSize = size });

                if (Random.value * context.Size.x < -5 + Vector2.Distance(currentPos, context.Center) / 1.5f) 
                    enemyPositions.Add(posInt);
            }
        }
    }

    private void DrawBrush(GenerationContext context, KeyPoint point)
    {
        int radius = Mathf.CeilToInt(point.AreaSize);
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int drawX = point.Position.x + x;
                int drawY = point.Position.y + y;

                if (drawX < 0 || drawX >= context.Size.x || drawY < 0 || drawY >= context.Size.y) continue;

                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist > point.AreaSize) continue;

                float influence = 1f - (dist / point.AreaSize);
                
                float currentValue = context.Map[drawX, drawY];
                context.Map[drawX, drawY] = Mathf.Max(currentValue, influence * 10f);
            }
        }
    }

    private void AddNoise(GenerationContext context)
    {
        float offset = Random.Range(-100f, 100f);
        for (int x = 0; x < context.Size.x; x++)
        {
            for (int y = 0; y < context.Size.y; y++)
            {
                if (context.Map[x, y] < 0.1f) continue;

                float noise = Mathf.PerlinNoise((x / scale) + offset, (y / scale) + offset);

                context.Map[x, y] += (noise * 2 - 1) * NoiseStrength;
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
