using UnityEngine;

public class PickUpableItem : Trigger
{
    public ItemBase Item;
    public override void OnEnter()
    {
        
    }

    public override void Activate()
    {
        Game.Inventory.NewItem(Item);
        Destroy(gameObject);
    }

    public override void OnExit()
    {
        
    }
}
