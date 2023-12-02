using UnityEngine;

public class CopySprite : MonoBehaviour
{
    private SpriteRenderer Sprite;
    public SpriteRenderer Reflection;
    void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Reflection.sprite = Sprite.sprite;
    }
}
