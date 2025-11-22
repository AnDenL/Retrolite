using UnityEngine;

[CreateAssetMenu(fileName = "CaveGenerator", menuName = "Game/Generators/CaveGenerator")]
public class CaveGenerator : MapGenerator
{
    [Header("Cellular Automata")]
    [Range(0, 100)] public int fillPercent = 45;
    public int smoothIterations = 5;

    [Header("Values")]
    public float wallValue = 0f;
    public float floorValue = 1f;

    public override float[,] Generate()
    {
        float[,] map = new float[Size.x, Size.y];

        System.Random rand = new System.Random();

        // початкове заповнення
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if (x == 0 || y == 0 || x == Size.x - 1 || y == Size.y - 1)
                    map[x, y] = wallValue; // край завжди стіна
                else
                    map[x, y] = (rand.Next(100) < fillPercent) ? wallValue : floorValue;
            }
        }

        // згладжування
        for (int i = 0; i < smoothIterations; i++)
        {
            map = Smooth(map);
        }

        return map;
    }

    private float[,] Smooth(float[,] map)
    {
        float[,] newMap = (float[,])map.Clone();

        for (int x = 1; x < Size.x - 1; x++)
        {
            for (int y = 1; y < Size.y - 1; y++)
            {
                int wallCount = GetWallCount(map, x, y);

                if (wallCount > 4)
                    newMap[x, y] = wallValue;
                else if (wallCount < 4)
                    newMap[x, y] = floorValue;
            }
        }

        return newMap;
    }

    private int GetWallCount(float[,] map, int cx, int cy)
    {
        int count = 0;
        for (int x = cx - 1; x <= cx + 1; x++)
        {
            for (int y = cy - 1; y <= cy + 1; y++)
            {
                if (!(x == cx && y == cy))
                    if (map[x, y] == wallValue) count++;
            }
        }
        return count;
    }
}
