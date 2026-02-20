using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Inventory
{
    static public Inventory Instance;

    public int maxSlots = 20;
    public List<ItemStack> items = new();
    public event Action<int> OnSlotChange;
    public event Action<ItemStack, int> OnNewSlot;
    public event Action OnInventoryChange;

    public Inventory() => Instance = this;

    public int AddItem(Item item, int count)
    {
        if (item.Stackable)
        {
            int index = items.FindIndex(stack => stack.Item.Id == item.Id && !stack.IsFull());
            ItemStack existing = null;
            if (index != -1)
                existing = items[index];

            if (existing != null)
            {
                int change = existing.Add(count);

                OnSlotChange?.Invoke(index);
                OnInventoryChange?.Invoke();
                return change;
            }
        }

        if (items.Count >= maxSlots)
            return 0;

        int added = Math.Min(count, item.MaxStack);
        ItemStack newItemStack = new ItemStack(item, added);
        items.Add(newItemStack);

        OnNewSlot?.Invoke(newItemStack, items.Count - 1);
        OnInventoryChange?.Invoke();
        return added;
    }

    public bool RemoveItem(Item item, int count)
    {
        int index = items.FindIndex(stack => stack.Item.Id == item.Id);
        ItemStack stack = null;

        if (index != -1)
            stack = items[index];
        else return false;

        if (stack.Count < count)
            return false;

        if (stack.Item.Stackable)
        {
            stack.Count -= count;
            if (stack.Count <= 0)
            {
                items.Remove(stack);
            }
            OnSlotChange?.Invoke(index);
        }
        else
        {
            items.Remove(stack);
        }
        OnInventoryChange?.Invoke();
        return true;
    }

    public int GetItemCount(Item item)
    {
        return items.Where(s => s.Item.Id == item.Id).Sum(s => s.Count);
    }
}


[Serializable]
public class ItemStack
{
    public Item Item;
    public int Count = 1;

    public ItemStack(Item item, int count)
    {
        Item = item;
        Count = count;
    }

    public bool IsFull() => Count == Item.MaxStack;

    public int Add(int value)
    {
        int previousCount = Count;
        Count = Math.Min(Count + value, Item.MaxStack);

        return Count - previousCount;
    }
}