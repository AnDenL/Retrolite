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

    public override void Generate(GenerationContext context)
    {
        System.Random rand = new();

        for (int x = 0; x < context.Size.x; x++)
        {
            for (int y = 0; y < context.Size.y; y++)
            {
                if (x == 0 || y == 0 || x == context.Size.x - 1 || y == context.Size.y - 1)
                    context.Map[x, y] = wallValue; 
                else if (Mathf.Abs(x - context.Size.x/2) + Mathf.Abs(y - context.Size.y/2)< 15)
                    context.Map[x, y] = floorValue;
                else
                    context.Map[x, y] = (rand.Next(100) < fillPercent) ? wallValue : floorValue;
            }
        }

        for (int i = 0; i < smoothIterations; i++)
        {
            context.Map = Smooth(context);
        }
    }

    private float[,] Smooth(GenerationContext context)
    {
        float[,] newMap = (float[,])context.Map.Clone();

        for (int x = 1; x < context.Size.x - 1; x++)
        {
            for (int y = 1; y < context.Size.y - 1; y++)
            {
                int wallCount = GetWallCount(context.Map, x, y);

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
