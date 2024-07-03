using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGeneration
{
    
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int StartPosition, int Length)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(StartPosition);
        Vector2Int previousPosition = StartPosition;

        for(int i = 0; i < Length; i++)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition; 
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int(0,1),
        new Vector2Int(1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
    };

    public static Vector2Int GetRandomDirection()
    {
        return Directions[Random.Range(0,Directions.Count)];
    }
}
