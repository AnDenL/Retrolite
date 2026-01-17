using UnityEngine;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoSizeBackground : MonoBehaviour
{
    public TextMeshPro textComponent;
    public SpriteRenderer spriteRenderer;
    public Vector2 padding = new(0.1f, 0.1f);

    private bool _isResizing = false;

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        RefreshComponents();
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void OnTextChanged(Object obj)
    {
        if (!_isResizing && obj == textComponent)
        {
            Resize();
        }
    }

    private void RefreshComponents()
    {
        if (textComponent == null && transform.parent != null)
            textComponent = transform.parent.GetComponent<TextMeshPro>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        Resize();
    }

    public void Resize()
    {
        if (textComponent == null || spriteRenderer == null) return;

        if (string.IsNullOrWhiteSpace(textComponent.text))
        {
            spriteRenderer.size = new Vector2();
            return;
        }

        _isResizing = true;
        
        textComponent.ForceMeshUpdate();
        var bounds = textComponent.textBounds;
        
        spriteRenderer.size = new Vector2(bounds.size.x + padding.x, bounds.size.y + padding.y);
        spriteRenderer.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, 0.01f);

        _isResizing = false;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying) 
        {
            Resize(); 
        }
    }
#endif
}