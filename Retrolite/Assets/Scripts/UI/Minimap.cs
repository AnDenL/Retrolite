using Creatures;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public Image Map, Fullmap;
    public Camera RenderCamera;

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
        if (Input.GetKeyDown(KeyCode.M)) Switch();
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

    public void Set(Vector2Int mapSize)
    {
        RenderTexture rt = new(mapSize.x, mapSize.y, 16)
        {
            filterMode = FilterMode.Point
        };

        RenderCamera.targetTexture = rt; 

        RenderCamera.gameObject.SetActive(true);
        RenderCamera.orthographicSize = mapSize.x / 2f;

        RenderCamera.Render();

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D texture2D = new(rt.width, rt.height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentActiveRT;
        RenderCamera.gameObject.SetActive(false);
        
        RenderCamera.targetTexture = null;
        rt.Release();

        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, mapSize.x, mapSize.y), new Vector2(0.5f, 0.5f), 16);
        Map.sprite = sprite;
        Fullmap.sprite = sprite;

        Map.transform.localScale = Vector2.one * currentZoom;
        Fullmap.transform.localScale = Map.transform.localScale * 4;
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
}