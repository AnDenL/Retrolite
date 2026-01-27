using UnityEngine;

[ExecuteAlways]
public class WeaponGenerator : MonoBehaviour
{
    private static WeaponGenerator _instance;
    public static WeaponGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<WeaponGenerator>();
            }
            return _instance;
        }
    }

    public SpriteList FisrtPart;
    public SpriteList SecondPart;
    public WeaponNameList Names;
    public SpriteList BulletList;

    private void Awake()
    {
        _instance = this;
    }

    public Sprite RandomSprite() => CombineSprites(FisrtPart.RandomSprite(), SecondPart.RandomSprite(), Random.value);
    public Sprite RandomSprite(GameRandom rnd) => CombineSprites(FisrtPart.RandomSprite(rnd), SecondPart.RandomSprite(rnd), rnd.Value);

    public static Sprite CombineSprites(Sprite part1, Sprite part2, float hueShift)
    {
        if (part1 == null || part2 == null) return null;

        Texture2D texture = new(34, 18, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            hideFlags = HideFlags.HideAndDontSave 
        };

        Color32[] clearPixels = new Color32[34 * 18]; 
        texture.SetPixels32(clearPixels);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 15; x++)
            {
                Color pixel = GetSpritePixel(part1, x, y);
                if (pixel.a > 0)
                {
                    ChangeHue(ref pixel, hueShift);
                    texture.SetPixel(x + 1, y + 1, pixel);
                }
            }
            for (int x = 0; x < 16; x++)
            {
                Color pixel = GetSpritePixel(part2, x, y);
                if (pixel.a > 0)
                {
                    ChangeHue(ref pixel, hueShift);
                    texture.SetPixel(x + 14, y + 1, pixel);
                }
            }
        }
        
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 34, 18), new Vector2(0.45f, 0.6f), 16);
    }

    private static Color GetSpritePixel(Sprite sprite, int x, int y)
    {
        if (sprite == null) return Color.clear;

        Texture2D texture = sprite.texture;
        Rect rect = sprite.textureRect;
        
        int pixelX = (int)(rect.x + x);
        int pixelY = (int)(rect.y + y);

        if (pixelX >= texture.width || pixelY >= texture.height) return Color.clear;

        return texture.GetPixel(pixelX, pixelY);
    }

    private static void ChangeHue(ref Color color, float hueShift)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = (h + hueShift) % 1f;
        float alpha = color.a;
        color = Color.HSVToRGB(h, s, v);
        color.a = alpha;
    }
}