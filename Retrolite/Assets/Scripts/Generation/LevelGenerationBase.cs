using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerationBase : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = new(64, 64);
    [SerializeField] private Layer[] layers;

    private GenerationContext context;

    private void Start()
    {
        Regenerate();
    }

    [ContextMenu("Regenerate")]
    private void Regenerate()
    {
        ClearMap();

        context = new GenerationContext(mapSize);

        foreach (var layer in layers)
        {
            RunLayer(layer);
        }
    }

    private void RunLayer(Layer layer)
    {
        foreach (var generator in layer.Generators)
        {
            if (generator == null) continue;
            generator.Generate(context);
        }

        RenderLayer(layer.MapTiles);
    }

    private void RenderLayer(MapTile[] mapTiles)
    {
        Vector2Int offset = context.Size / 2;

        for (int y = 0; y < context.Size.y; y++)
        {
            for (int x = 0; x < context.Size.x; x++)
            {
                float value = context.Map[x, y];
                Vector3Int pos = new(x - offset.x, y - offset.y, 0);
                SetTile(mapTiles, pos, value);
            }
        }
    }

    private void SetTile(MapTile[] mapTiles, Vector3Int pos, float value)
    {
        foreach (var tile in mapTiles)
        {
            if (value >= tile.MinValue)
            {
                tile.Layer?.SetTile(
                    pos,
                    tile.Tiles[Random.Range(0, tile.Tiles.Length)]
                );
                break;
            }
        }
    }

    private void ClearMap()
    {
        foreach (var layer in layers)
        {
            foreach (var tile in layer.MapTiles)
            {
                tile.Layer?.ClearAllTiles();
            }
        }
    }
}

[System.Serializable]
public class MapTile
{
    public Tilemap Layer;
    [Tooltip("Minimum value to use this tile (inclusive).")]
    public float MinValue;
    public TileBase[] Tiles;
}

[System.Serializable]
public class Layer
{
    [Header("Map Generator")]
    public MapGenerator[] Generators;

    [Header("Tile Settings")]
    public MapTile[] MapTiles;
}

public abstract class MapGenerator : ScriptableObject
{
    public abstract void Generate(GenerationContext context);
}

public class GenerationContext
{
    public float[,] Map;
    public Vector2Int Size;

    public GenerationContext(Vector2Int size)
    {
        Size = size;
        Map = new float[size.x, size.y];
    }
}
