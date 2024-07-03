using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleDungeonGenerator : AbstractGenerator
{
    [SerializeField] protected Dungeon _preference;

    [ContextMenu("Generate")]
    protected override void Generate()
    {
        HashSet<Vector2Int> floorPositions = RandomWalk(_preference, _startPosition);
        _generator.Clear();
        _generator.PaintTiles(floorPositions);
        WallsGenerator.CreateWalls(floorPositions, _generator);
    }

    protected HashSet<Vector2Int> RandomWalk(Dungeon preference,Vector2Int position)
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPosition = new HashSet<Vector2Int>();

        for(int i = 0; i < preference.Iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGeneration.SimpleRandomWalk(currentPosition, preference.WalkLength);
            floorPosition.UnionWith(path);

            if(preference.StartRandomly)
                currentPosition = floorPosition.ElementAt(Random.Range(0, floorPosition.Count));
        }

        return floorPosition;
    }
}
