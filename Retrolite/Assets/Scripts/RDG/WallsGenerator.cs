using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallsGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapGenerator tilemapGenerator)
    {
        HashSet<Vector2Int> basicWalls = FindWallsInDirections(floorPositions, Direction2D.Directions);

        foreach(Vector2Int position in basicWalls)
        {
            tilemapGenerator.PaintWallTile(position);
        }
    } 

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach(Vector2Int position in floorPositions)
        {
            foreach(Vector2Int direction in directions)
            {
                Vector2Int neighbour = position + direction;
                if(!floorPositions.Contains(neighbour))
                    wallPositions.Add(neighbour);
            }
        }

        return wallPositions;
    }
}
