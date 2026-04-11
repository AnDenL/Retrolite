using System;
using UnityEngine;

public class Shop : MonoBehaviour, IGenerationStruct
{
    [Header("Items")]
    [SerializeField] private GameObject shopSlot;
    [SerializeField] private ForSaleItem[] items;
    [SerializeField] private int minCount, maxCount;

    [Header("Look")]
    [SerializeField] private FloorGenerator floor;
    [SerializeField] private SpriteRenderer table;
    [SerializeField] private GameObject[] decorations;
    [SerializeField] private Transform point;
    [SerializeField] private int minDecorationsCount, maxDecorationsCount;

    [ContextMenu("Generate")]
    public void GenerateExample() => Generate(new GameRandom((uint)UnityEngine.Random.Range(1, 12000)));

    public void Generate(GameRandom random)
    {
        Clear();

        int count = random.Range(minCount, maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            ForSaleItem item = items[random.Range(0, items.Length)];
            ShopItem shopItem = Instantiate(shopSlot, table.transform).GetComponent<ShopItem>();
            shopItem.Item = Instantiate(item.Prefab, shopItem.transform).transform;
            shopItem.price = random.Range(item.minCost, item.maxCost);
            shopItem.transform.localPosition = new Vector3(i * 1.25f - count * 0.625f + 0.625f, 0.5f, -0.5f);
        }

        int deco = random.Range(minDecorationsCount, maxDecorationsCount);
        for (int i = 0; i < deco; i++)
        {
            Vector2 pos = random.PointInCircle(1) * new Vector2(5, 3);

            GameObject pref = decorations[random.Range(0, decorations.Length)];

            if (pref.TryGetComponent(out Collider2D collider))
            {
                if (Physics2D.OverlapCircleAll(pos, 2f).Length != 0) 
                    continue;
            }

            GameObject spawned = Instantiate(pref, point);
            spawned.transform.localPosition = pos;
        }

        table.size = new Vector2(0.5f + count * 1.25f, table.size.y);
    }

    public void Clear()
    {
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
