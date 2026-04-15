using UnityEngine;

public class TreeGenerator : MonoBehaviour, IGenerationStruct
{
    [ContextMenu("Generate")]
    public void Sample() => Generate(new GameRandom((uint)Random.Range(1,12000)));

    public void Generate(GameRandom random)
    {
        var line = GetComponent<LineRenderer>();
        float widthMultiplier = random.Range(1.5f, 2.5f);
        line.widthMultiplier = widthMultiplier;

        int count = random.Range(6, 9);

        line.positionCount = count;

        float dist = (4f + widthMultiplier) / count;
        Vector2 current = new();
        for (int i = 0; i < count; i++)
        {
            current += random.PointInCircle(0.1f) + Vector2.up * dist;
            line.SetPosition(i, current);
        }
    }

    public void Clear()
    {
        
    }
}
