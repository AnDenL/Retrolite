using CalculatingSystem;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ItemPickUp : Interactable
{
    public Item Item;
    public TextMeshPro Description;

    protected override void Start()
    {
        base.Start();

        sr.sprite = Item.Icon;
        Description.text = Item.ItemName + "\n" + (string.IsNullOrWhiteSpace(Item.CustomDescription) ? 
                            Item.Action.ToReadableString() : Item.CustomDescription);
    }
    
    public override void Interact(Creature creature)
    {
        Item.Activate(new Context()
        {
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