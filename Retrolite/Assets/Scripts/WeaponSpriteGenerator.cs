using UnityEngine;

public class WeaponSpriteGenerator : MonoBehaviour
{
    public static WeaponSpriteGenerator instance;
    public SpriteList List;

    private void Awake()
    {
        instance = this;
    }

    public Sprite RandomSprite() => CombineSprites(List.Parts1[Random.Range(0, List.Parts1.Length - 1)], List.Parts2[Random.Range(0, List.Parts2.Length - 1)], Random.Range(0f, 1f));

    public static Sprite CombineSprites(Sprite part1, Sprite part2, float hueShift)
    {
        Texture2D texture = new Texture2D(34, 18, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, clear);
            }
        }
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 15; x++)
            {
                Color pixel = GetSpritePixel(part1, x, y);

                ChangeHue(ref pixel, hueShift);
                texture.SetPixel(x + 1, y + 1, pixel);
            }
            for (int x = 0; x < 16; x++)
            {
                Color pixel = GetSpritePixel(part2, x, y);

                if (pixel.a == 0) continue;

                ChangeHue(ref pixel, hueShift);
                texture.SetPixel(x + 14, y + 1, pixel);
            }
        }
        texture.Apply();

        Sprite final = Sprite.Create(texture, new Rect(0, 0, 34, 18), new Vector2(0.45f, 0.6f), 16);

        return final;
    }

    private static Color GetSpritePixel(Sprite sprite, int x, int y)
    {
        Texture2D texture = sprite.texture;

        Rect rect = sprite.textureRect;
        int pixelX = (int)(rect.x + x);
        int pixelY = (int)(rect.y + y);

        return texture.GetPixel(pixelX, pixelY);
    }

    private static void ChangeHue(ref Color color, float hueShift)
    {
        float alpha = color.a;
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h += hueShift;
        if (h > 1f) h -= 1f;
        color = Color.HSVToRGB(h, s, v);
        color.a = alpha;
    }
}

[CreateAssetMenu(fileName = "SpriteList", menuName = "Game/SpriteList")]
public class SpriteList : ScriptableObject
{
    public Sprite[] Parts1, Parts2;
}