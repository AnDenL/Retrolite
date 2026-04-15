using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

using Random = Unity.Mathematics.Random;

public class LevelPlacer : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = new(64, 64);
    [SerializeField] private Layer[] layers;
    [SerializeField] private GameObject[] keyStructs;
    [SerializeField] private Minimap map;
    public uint seed;

    private GenerationContext context;

    private GameRandom Random => context.Random;

    private void Start()
    {
        seed = (uint)System.Guid.NewGuid().GetHashCode();

        Regenerate();

        if(map) map.Set(mapSize);
    }

    [ContextMenu("Regenerate")]
    private void Regenerate()
    {
        ClearMap();

        context = new GenerationContext(mapSize, keyStructs.Length, seed);

        foreach (var layer in layers)
        {
            RunLayer(layer);
        }

        for (int i = 0; i < keyStructs.Length; i++)
        {
            Vector2 pos = context.KeyStructs[i] - mapSize / 2;
            GameObject obj = Instantiate(keyStructs[i], new Vector3(pos.x, pos.y), Quaternion.identity);

            obj.transform.parent = transform;
            obj.transform.position = new Vector3(pos.x, pos.y, keyStructs[i].transform.position.z);
            if (obj.TryGetComponent<IGenerationStruct>(out var gen)) gen.Generate(Random); 
        }
    }

    private void RunLayer(Layer layer)
    {
        foreach (var generator in layer.Generators)
        {
            if (generator == null) continue;
            generator.Generate(context);
        }

        foreach (var pos in context.Enemies)
        {
            if (Physics2D.OverlapCircleAll(pos, 2f).Length != 0) continue;
            Instantiate(layer.Enemies[Random.Range(0, layer.Enemies.Length)], new Vector3(pos.x - mapSize.x / 2, pos.y - mapSize.y / 2), Quaternion.identity).transform.parent = transform;
        }
        foreach (var pos in context.Structs)
        {
            if (Physics2D.OverlapCircleAll(pos, 2f).Length != 0) continue;

            GameObject prefab = layer.Structs[Random.Range(0, layer.Structs.Length)];
            GameObject obj = Instantiate(prefab, new Vector3(pos.x - mapSize.x / 2, pos.y - mapSize.y / 2), Quaternion.identity);
            obj.transform.parent = transform;
            if (obj.TryGetComponent<IGenerationStruct>(out var gen)) gen.Generate(Random); 
            obj.transform.position += prefab.transform.position;
        }

        RenderLayer(layer.MapTiles);
    }

    private void RenderLayer(MapTile[] mapTiles)
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

        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
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
    public GameObject[] Enemies;
    public GameObject[] Structs;
}

public abstract class MapGenerator : ScriptableObject
{
    public abstract void Generate(GenerationContext context);
}

public class GenerationContext
{
    public float[,] Map;
    public GameRandom Random;
    public Vector2Int Size;
    public Vector2Int Center;
    public List<Vector2Int> Enemies;
    public List<Vector2Int> Structs;
    public Vector2Int[] KeyStructs;

    public GenerationContext(Vector2Int size, int keyStructs, uint seed)
    {
        Size = size;
        Center = size / 2;
        Map = new float[size.x, size.y];
        Enemies = new();
        Structs = new();
        KeyStructs = new Vector2Int[keyStructs];
        Random = new(seed);
    }
}
