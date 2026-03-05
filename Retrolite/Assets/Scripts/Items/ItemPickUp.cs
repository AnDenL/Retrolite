using CalculatingSystem;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ItemPickUp : Interactable
{
    public ItemStack Stack;
    public TextMeshPro Description;

    protected override void Start()
    {
        base.Start();

        Set();
    }
    
    public void Set()
    {
        sr.sprite = Stack.Item.Icon;
        Description.text = Stack.Item.ItemName + "\n" + (string.IsNullOrWhiteSpace(Stack.Item.CustomDescription) ? 
                            Stack.Item.Action.ToReadableString() : Stack.Item.CustomDescription);
    }

    public void Set(ItemStack stack)
    {
        Stack = stack;
        Set();
    }
    
    public override void Interact(Creature creature)
    {
        Stack.Count -= creature.AddItem(Stack);
        
        if (Stack.Count <= 0) Destroy(gameObject);
    }

    public void Use(Creature creature)
    {
        Stack.Item.Activate(new Context()
        {
            Target = creature, 
            Owner = creature
        });

        if (Stack.Item.Sound) creature.PlaySound(Stack.Item.Sound);
        if (Stack.Item.SingleUse) Stack.Count--;
        if (Stack.Count <= 0) Destroy(gameObject);
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