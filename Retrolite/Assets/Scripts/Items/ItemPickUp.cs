using CalculatingSystem;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ItemPickUp : Interactable
{
    public Item Item;
    public TextMesh Description;

    private AudioSource source;

    private void Start()
    {
        sr.sprite = Item.Icon;
        Description.text = Item.ItemName + "\n" + Item.Action.ToReadableString();
        source = GetComponent<AudioSource>();
    }
    
    public override void Interact(Creature creature)
    {
        Item.Activate(new Context()
        {
            TargetHealth = creature.HealthComponent, 
            Target = creature, 
            Owner = creature
        });

        if (Item.Sound) creature.PlaySound(Item.Sound);
        
        if (Item.SingleUse) Destroy(gameObject);
    }

    public override void Outline()
    {
        base.Outline();    
        Description.gameObject.SetActive(true);
    }

    public override void CancelOutline()
    {
        base.CancelOutline();
        Description.gameObject.SetActive(false);
    }
}