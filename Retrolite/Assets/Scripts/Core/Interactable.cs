using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected Material outlineMaterial;

    protected SpriteRenderer sr;
    protected Material defaultMaterial;

    protected void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;
        var newMaterial = new Material(outlineMaterial);
        newMaterial.SetTexture("_MainTex", sr.sprite.texture);
        outlineMaterial = newMaterial;
    }

    public virtual void Outline() { sr.material = outlineMaterial; }
    public virtual void CancelOutline() { sr.material = defaultMaterial; }
    public virtual void Interact(Creature creature) { }
}
