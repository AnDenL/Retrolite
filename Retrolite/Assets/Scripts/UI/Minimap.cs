using Creatures;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour
{
    public Image Map, Fullmap;
    public Camera RenderCamera;
    public RenderTexture MapTexture;

    private bool isOpened;
    private float currentZoom = 4;
    private Vector2 fullMapPos;
    private Vector2 startDragMouse, startDragPos;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Map.transform.localScale = Vector2.one * currentZoom;
        Fullmap.transform.localScale = Map.transform.localScale * 4;
    }

    private void Switch()
    {
        isOpened = !isOpened;

        animator.SetBool("Opened", isOpened);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) Switch();
        if (isOpened)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragMouse = Input.mousePosition;
                startDragPos = fullMapPos;
            }
            else if (Input.GetMouseButton(0))
            {
                fullMapPos = startDragPos + ((Vector2)Input.mousePosition - startDragMouse);
            }

            Fullmap.transform.position = fullMapPos;
        }

        Map.transform.localPosition = -PlayerController.Player.transform.position * currentZoom;
    }

    public void Set(GenerationContext context)
    {
        RenderCamera.orthographicSize = context.Size.x / 2;

        MapTexture.texelSize.Set(context.Size.x, context.Size.y);
        
        RenderCamera.Render();

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = MapTexture;

        Texture2D texture2D = new(MapTexture.width, MapTexture.height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        texture2D.ReadPixels(new Rect(0, 0, MapTexture.width, MapTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentActiveRT;

        Map.sprite = Sprite.Create(texture2D, new Rect(0, 0, context.Size.x, context.Size.y), new Vector2(0.5f, 0.5f), 16);
        Fullmap.sprite = Map.sprite;
    }

    public void Zoom()
    {
        if (Input.mouseScrollDelta.y == 0) return;

        float previousZoom = currentZoom;

        currentZoom += Input.mouseScrollDelta.y > 0 ? 0.5f : -0.5f;
        currentZoom = Mathf.Clamp(currentZoom, 1, 8);
        Map.transform.localScale = Vector2.one * currentZoom;
        Fullmap.transform.localScale = Map.transform.localScale * 4;

        Vector2 halfScreen = new Vector2(Screen.width, Screen.height) / 2;

        fullMapPos = (fullMapPos -halfScreen) / previousZoom * currentZoom;
        fullMapPos += halfScreen;
        Fullmap.transform.position = fullMapPos;
    }

    /*
    public void Set(GenerationContext context)
    {
        Texture2D tex = new(context.Size.x, context.Size.y)
        {
            filterMode = FilterMode.Point
        };

        Color32[] pixels = new Color32[context.Size.x * context.Size.y];

        for (int y = 0; y < context.Size.y; y++)
        {
            for (int x = 0; x < context.Size.y; x++)
            {
                float val = context.Map[x, y];
                
                if (val > 1.0f) 
                    pixels[y * context.Size.x + x] = Color.white;
                else 
                    pixels[y * context.Size.x + x] = Color.clear;
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        Map.sprite = Sprite.Create(tex, new Rect(0, 0, context.Size.x, context.Size.y), new Vector2(0.45f, 0.6f), 16);
        Map.SetNativeSize();
    }

    public void GetColor()
    {
        
    }
    */
}