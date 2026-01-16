using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemDrawerEditor : Editor
{
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        Item item = (Item)target;

        if (item == null || item.Icon == null)
            return base.RenderStaticPreview(assetPath, subAssets, width, height);

        Texture2D newIcon = new(width, height);
        
        Sprite sprite = item.Icon;
        Texture2D spriteTexture = AssetPreview.GetAssetPreview(sprite);

        return spriteTexture;
    }
}