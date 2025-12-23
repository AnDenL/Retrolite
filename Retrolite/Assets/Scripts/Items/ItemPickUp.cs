using CalculatingSystem;
using UnityEngine;

public class ItemPickUp : Interactable
{
    public Item Item;

    private void Start() => sr.sprite = Item.Icon;
    
    public override void Interact(Creature creature)
    {

    }
}