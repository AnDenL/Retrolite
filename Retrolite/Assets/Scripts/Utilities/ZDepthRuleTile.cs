using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/ZDepth Rule Tile")]
public class ZDepthRuleTile : RuleTile<ZDepthRuleTile.Neighbor>
{
    public float offset;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        tileData.transform = Matrix4x4.TRS(
            new Vector3(0, 0, position.y + offset),
            Quaternion.identity,
            Vector3.one
        );
    }

    public class Neighbor : RuleTile.TilingRule.Neighbor {}
}
