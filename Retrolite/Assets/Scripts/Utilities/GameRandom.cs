using UnityEngine;
using Unity.Mathematics;

public class GameRandom
{
    private Unity.Mathematics.Random rng;

    public GameRandom(uint seed)
    {
        rng = new Unity.Mathematics.Random(seed == 0 ? 1 : seed);
    }

    public uint NextUInt() => rng.NextUInt();

    public int Range(int min, int max) => rng.NextInt(min, max);
    public float Range(float min, float max) => rng.NextFloat(min, max);
    public float Value => rng.NextFloat();
    public bool Chance(float probability) => rng.NextFloat() < probability;

    public Vector2 PointInCircle(float radius)
    {
        float2 dir = rng.NextFloat2Direction();
        float r = math.sqrt(rng.NextFloat()) * radius;
        return new Vector2(dir.x * r, dir.y * r);
    }
    
    public Vector2 Direction()
    {
        float2 dir = rng.NextFloat2Direction();
        return new Vector2(dir.x, dir.y);
    }

    public Vector2Int GetRandomDirection() => rng.NextInt(0, 6) switch
    {
        0 => Vector2Int.left,
        1 => Vector2Int.right,
        2 => Vector2Int.up,
        3 => Vector2Int.down,
        4 => Vector2Int.left,
        5 => Vector2Int.right,
        _ => Vector2Int.zero,
    };
}