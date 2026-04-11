using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorGenerator : MonoBehaviour, IGenerationStruct
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private int minSize, maxSize;

    [ContextMenu("Generate")]
    public void GenerateExample() => Generate(new GameRandom((uint)UnityEngine.Random.Range(1, 12000)));

    public void Generate(GameRandom random)
    {
        Clear();
        HashSet<Vector3Int> positions = GenerateFloor(random);
        TileBase[] tiles = new TileBase[positions.Count];
        Array.Fill(tiles, tile);
        tilemap.SetTiles(positions.ToArray(), tiles);
    }

    public HashSet<Vector3Int> GenerateFloor(GameRandom random)
    {
        HashSet<Vector3Int> positions = new();

        int size = random.Range(minSize, maxSize+1);

        Vector3Int position = new();
        positions.Add(position);

        for (int i = 0; i < size; i++)
        {
            position += (Vector3Int)random.GetRandomDirection();

            if (positions.Contains(position))
            {
                i--;
                continue;
            }
            positions.Add(position);
            positions.Add(position + Vector3Int.left);

            if (i % 8 == 7 || position.magnitude > minSize / 3) position = Vector3Int.zero;
        }

        return positions;
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}