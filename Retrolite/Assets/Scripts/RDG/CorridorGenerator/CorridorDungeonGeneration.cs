using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorDungeonGeneration : SimpleDungeonGenerator
{
    [SerializeField] private int _corridorLenght, _corridorCount;
    [SerializeField] [Range(0f,1f)] private float _roomPercent = 0.8f;
    
    protected override void Generate()
    {
        CorridorGeneration();
    }
    [ContextMenu("CorridorGeneration")]
    private void CorridorGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        
        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindDeadEnds(floorPositions); 

        CreateRoomsAtEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        _generator.Clear();
        _generator.PaintTiles(floorPositions);
        WallsGenerator.CreateWalls(floorPositions, _generator);
    }

    private List<Vector2Int> FindDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (Vector2Int position in floorPositions)
        {
            int neigboursCount = 0;

            foreach (Vector2Int direction in Direction2D.Directions)
                if(floorPositions.Contains(position + direction)) neigboursCount++;

            if(neigboursCount == 1) deadEnds.Add(position);
        }
        return deadEnds;
    }

    private void CreateRoomsAtEnds(List<Vector2Int> deadEnds,HashSet<Vector2Int> roomPositions)
    {
        foreach (Vector2Int position in deadEnds)
        {
            if(!roomPositions.Contains(position))
            {
                HashSet<Vector2Int> room = RandomWalk(_preference, position);
                roomPositions.UnionWith(room);
            }
        }
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * _roomPercent);

        List<Vector2Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (Vector2Int roomPosition in roomToCreate)
        {
            HashSet<Vector2Int> room = RandomWalk(_preference, roomPosition);
            roomPositions.UnionWith(room);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int currentPosition = _startPosition;
        potentialRoomPositions.Add(currentPosition);

        for(int i = 0; i < _corridorCount; i++)
        {
            List<Vector2Int> path = ProceduralGeneration.RandomWalkCorridor(currentPosition, _corridorLenght);

            currentPosition = path[path.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(path);
        }
    }
}
