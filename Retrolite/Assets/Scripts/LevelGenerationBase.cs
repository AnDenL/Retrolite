using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerationBase : MonoBehaviour
{
    [Header("Map Generator")]
    [SerializeField] private MapGenerator generator;

    [Header("Tile Settings")]
    [SerializeField] private MapTile[] mapTiles;

    private float[,] map;

    private void Awake()
    {
        System.Array.Sort(mapTiles, (a, b) => b.MinValue.CompareTo(a.MinValue));
    }

    private void Start()
    {
        Regenerate();
    }

    [ContextMenu("Regenerate")]
    private void Regenerate()
    {
        ClearMap();
        Generate();
    }

    private void Generate()
    {
        if (generator == null)
        {
            Debug.LogError("Map generation asset not assigned.");
            return;
        }

        map = generator.Generate();

        if (map == null)
        {
            Debug.LogError("Generated map is null.");
            return;
        }

        PlaceMap();
    }

    private void PlaceMap()
    {
        Vector2Int startPosition = new Vector2Int(map.GetLength(0) / 2, map.GetLength(1) / 2);

        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                SetTile(new Vector3Int(x - startPosition.x, y - startPosition.y, 0), map[x, y]);
            }
        }
    }

    private void SetTile(Vector3Int pos, float value)
    {
        foreach (var tile in mapTiles)
        {
            if (value >= tile.MinValue)
            {
                tile.Layer?.SetTile(pos, tile.Tiles[Random.Range(0, tile.Tiles.Length)]);
                break;
            }
        }
    }

    [ContextMenu("ClearMap")]
    private void ClearMap()
    {
        foreach (var tile in mapTiles)
        {
            tile.Layer?.ClearAllTiles();
        }
    }
}

[System.Serializable]
public class MapTile
{
    public Tilemap Layer;
    public Tile[] Tiles;
    [Tooltip("Minimum value to use this tile (inclusive).")]
    public float MinValue;
}

public abstract class MapGenerator : ScriptableObject
{
    public abstract float[,] Generate();
}
