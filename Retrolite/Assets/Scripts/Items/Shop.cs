using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour, IGenerationStruct
{
    [Header("Items")]
    [SerializeField] private GameObject shopSlot;
    [SerializeField] private ForSaleItem[] items;
    [SerializeField] private int minCount, maxCount;

    [Header("Look")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private int minSize, maxSize;
    [SerializeField] private SpriteRenderer table;
    [SerializeField] private GameObject[] decorations;
    [SerializeField] private Transform point;
    [SerializeField] private int minDecorationsCount, maxDecorationsCount;

    [ContextMenu("Generate")]
    public void Generate()
    {
        Clear();
        HashSet<Vector3Int> positions = GenerateFloor();
        TileBase[] tiles = new TileBase[positions.Count];
        Array.Fill(tiles, tile);
        tilemap.SetTiles(positions.ToArray(), tiles);

        int count = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            ForSaleItem item = items[Random.Range(0, items.Length)];
            ShopItem shopItem = Instantiate(shopSlot, table.transform).GetComponent<ShopItem>();
            shopItem.Item = Instantiate(item.Prefab, shopItem.transform).transform;
            shopItem.price = Random.Range(item.minCost, item.maxCost);
            shopItem.transform.localPosition = new Vector3(i * 1.25f - count * 0.625f + 0.625f, 0.5f, -0.1f);
        }

        int deco = Random.Range(minDecorationsCount, maxDecorationsCount);
        for (int i = 0; i < deco; i++)
        {
            Vector2 pos = Random.insideUnitCircle * new Vector2(5, 3);

            GameObject pref = decorations[Random.Range(0, decorations.Length)];

            if (pref.TryGetComponent(out Collider2D collider))
            {
                if (Physics2D.OverlapCircleAll(pos, 2f).Length != 0) 
                    continue;
            }

            GameObject spawned = Instantiate(pref, point);
            spawned.transform.localPosition = pos;
        }

        table.size = new Vector2(1f + count * 1.25f, table.size.y);
    }

    public HashSet<Vector3Int> GenerateFloor()
    {
        HashSet<Vector3Int> positions = new();

        int size = Random.Range(minSize, maxSize+1);

        Vector3Int position = new();
        positions.Add(position);

        for (int i = 0; i < size; i++)
        {
            position += GetRandomDirection();

            if (positions.Contains(position))
            {
                i--;
                continue;
            }
            positions.Add(position);
            positions.Add(position + Vector3Int.left);

            if (i % 8 == 7 || position.magnitude > minSize / 3) position = Vector3Int.zero;
        }

        return positions;
    }

    public Vector3Int GetRandomDirection() => Random.Range(0, 6) switch
    {
        0 => Vector3Int.left,
        1 => Vector3Int.right,
        2 => Vector3Int.up,
        3 => Vector3Int.down,
        4 => Vector3Int.left,
        5 => Vector3Int.right,
        _ => Vector3Int.zero,
    };

    public void Clear()
    {
        tilemap.ClearAllTiles();
        while (table.transform.childCount > 0)
            DestroyImmediate(table.transform.GetChild(0).gameObject);
        while (point.transform.childCount > 0)
            DestroyImmediate(point.transform.GetChild(0).gameObject);
    }
}

[Serializable]
public struct ForSaleItem
{
    public GameObject Prefab;
    public int minCost;
    public int maxCost;
}
