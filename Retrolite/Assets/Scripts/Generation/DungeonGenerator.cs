using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DungeonGenerator", menuName = "Game/Generators/DungeonGenerator")]
public class DungeonGenerator : MapGenerator
{
    [Header("Rooms")]
    public int roomCount = 8;
    public Vector2Int roomMinSize = new Vector2Int(4, 4);
    public Vector2Int roomMaxSize = new Vector2Int(10, 10);

    [Header("Values")]
    public float wallValue = 0f;
    public float floorValue = 1f;

    public override float[,] Generate()
    {
        float[,] map = new float[Size.x, Size.y];

        // спочатку все робимо стінами
        for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
                map[x, y] = wallValue;

        List<RectInt> rooms = new List<RectInt>();

        // створюємо кімнати
        for (int i = 0; i < roomCount; i++)
        {
            int w = Random.Range(roomMinSize.x, roomMaxSize.x + 1);
            int h = Random.Range(roomMinSize.y, roomMaxSize.y + 1);
            int x = Random.Range(1, Size.x - w - 1);
            int y = Random.Range(1, Size.y - h - 1);

            RectInt newRoom = new RectInt(x, y, w, h);
            bool overlaps = false;

            foreach (var r in rooms)
            {
                if (newRoom.Overlaps(r)) { overlaps = true; break; }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);
                CarveRoom(map, newRoom);
            }
        }

        // з’єднуємо кімнати коридорами
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int prevCenter = Vector2ToInt(rooms[i - 1].center);
            Vector2Int currCenter = Vector2ToInt(rooms[i].center);

            CarveCorridor(map, prevCenter, currCenter);
        }

        return map;
    }

    private Vector2Int Vector2ToInt(Vector2 v) => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

    private void CarveRoom(float[,] map, RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                map[x, y] = floorValue;
            }
        }
    }

    private void CarveCorridor(float[,] map, Vector2Int from, Vector2Int to)
    {
        // L-подібний коридор
        if (Random.value < 0.5f)
        {
            CarveLine(map, from.x, to.x, from.y, true);
            CarveLine(map, from.y, to.y, to.x, false);
        }
        else
        {
            CarveLine(map, from.y, to.y, from.x, false);
            CarveLine(map, from.x, to.x, to.y, true);
        }
    }

    private void CarveLine(float[,] map, int start, int end, int fixedCoord, bool horizontal)
    {
        if (start > end) (start, end) = (end, start);

        for (int i = start; i <= end; i++)
        {
            if (horizontal) map[i, fixedCoord] = floorValue;
            else map[fixedCoord, i] = floorValue;
        }
    }
}
