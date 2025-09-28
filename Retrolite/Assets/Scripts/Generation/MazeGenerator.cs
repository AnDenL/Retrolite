using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MazeGenerator", menuName = "Generators/MazeGenerator")]
public class MazeGenerator : MapGenerator
{
    [Header("Values")]
    public float wallValue = 0f;
    public float floorValue = 1f;

    public override float[,] Generate()
    {
        float[,] map = new float[Size.x, Size.y];

        // все стіни
        for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
                map[x, y] = wallValue;

        Vector2Int start = new Vector2Int(1, 1);
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);

        map[start.x, start.y] = floorValue;

        System.Random rand = new System.Random();

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetNeighbors(current, map);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[rand.Next(neighbors.Count)];
                Vector2Int wall = (current + next) / 2;
                map[next.x, next.y] = floorValue;
                map[wall.x, wall.y] = floorValue;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        return map;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell, float[,] map)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        Vector2Int[] dirs = { Vector2Int.up * 2, Vector2Int.down * 2, Vector2Int.left * 2, Vector2Int.right * 2 };

        foreach (var dir in dirs)
        {
            Vector2Int n = cell + dir;
            if (n.x > 0 && n.y > 0 && n.x < Size.x - 1 && n.y < Size.y - 1)
            {
                if (map[n.x, n.y] == wallValue)
                    result.Add(n);
            }
        }

        return result;
    }
}
