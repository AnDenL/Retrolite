using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelGenerationBase : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = new(64, 64);
    [SerializeField] private Layer[] layers;

    [SerializeField] private GameObject portal;

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

        portal.transform.position = new Vector3(context.EndPoint.x - context.Size.x/2, context.EndPoint.y - context.Size.y/2);
    }

    private void RunLayer(Layer layer)
    {
        foreach (var generator in layer.Generators)
        {
            if (generator == null) continue;
            generator.Generate(context);
        }

        RenderLayerOptimized(layer.MapTiles);
    }

    private void RenderLayerOptimized(MapTile[] mapTiles)
    {
        Vector2Int size = context.Size;
        Vector2Int offset = size / 2;

        foreach (var mapTile in mapTiles)
        {
            if (mapTile.Layer == null)
                continue;

            RenderSingleTilemap(mapTile, mapTiles, size, offset);
        }
    }

    private void RenderSingleTilemap(
        MapTile target,
        MapTile[] allTiles,
        Vector2Int size,
        Vector2Int offset)
    {
        Tilemap tilemap = target.Layer;

        var positions = new List<Vector3Int>();
        var tiles = new List<TileBase>();

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                float value = context.Map[x, y];

                MapTile chosen = null;
                foreach (var tile in allTiles)
                {
                    if (value >= tile.MinValue)
                    {
                        chosen = tile;
                        break;
                    }
                }

                if (chosen != target)
                    continue;

                Vector3Int pos = new(
                    x - offset.x,
                    y - offset.y,
                    0
                );

                positions.Add(pos);
                tiles.Add(
                    target.Tiles[Random.Range(0, target.Tiles.Length)]
                );
            }
        }

        tilemap.gameObject.SetActive(false);
        tilemap.SetTiles(positions.ToArray(), tiles.ToArray());
        tilemap.gameObject.SetActive(true);
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
                if (tile.Layer == null) continue;

                tile.Layer.gameObject.SetActive(false);
                tile.Layer.ClearAllTiles();
                tile.Layer.gameObject.SetActive(true);
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
    public Vector2Int EndPoint;

    public GenerationContext(Vector2Int size)
    {
        Size = size;
        Map = new float[size.x, size.y];
    }
}
