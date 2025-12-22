using UnityEngine;

[CreateAssetMenu(fileName = "IslandGenerator", menuName = "Game/Generators/IslandGenerator")]
public class IslandGenerator : MapGenerator
{
    [Header("Map Settings")]
    public float scale = 0.1f;

    [Header("Values")]
    public float waterValue = 0f;
    public float landValue = 1f;

    public override void Generate(GenerationContext context)
    {
        Vector2 center = new Vector2(context.Size.x / 2f, context.Size.y / 2f);
        float maxDist = center.magnitude;

        for (int x = 0; x < context.Size.x; x++)
        {
            for (int y = 0; y < context.Size.y; y++)
            {
                float nx = x * scale;
                float ny = y * scale;

                float noise = Mathf.PerlinNoise(nx, ny);

                float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                noise -= dist * 0.5f;

                context.Map[x, y] = noise > 0 ? landValue : waterValue;
            }
        }
    }
}
