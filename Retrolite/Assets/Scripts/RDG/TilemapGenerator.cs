using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap, _wallTilemap;
    [SerializeField] private TileBase _tiles, _wallTop;

    public void PaintTiles(IEnumerable<Vector2Int> positions)
    {
        PaintLayerTiles(positions, _tilemap, _tiles);
    }

    internal void PaintWallTile(Vector2Int position)
    {
        Paint(position, _wallTilemap, _wallTop);
    }

    private void PaintLayerTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach(Vector2Int position in positions)
        {
            Paint(position, tilemap, tile);
        }
    }

    private void Paint(Vector2Int position, Tilemap tilemap, TileBase tile)
    {
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        _tilemap.ClearAllTiles();
        _wallTilemap.ClearAllTiles();
    }
}
