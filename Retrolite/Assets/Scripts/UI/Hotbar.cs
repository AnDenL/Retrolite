using System.Collections.Generic;
using Creatures;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform bar;
    
    private readonly List<Item> hotbarItems = new();
    private readonly List<ItemSlot> hotbarItemSlots = new();
    private readonly Dictionary<int, ItemSlot> hotbarSlots = new();

    private void Awake() => Instance = this;

    private void Start()
    {
        PlayerController.Player.Inventory.OnSlotChange += UpdateUI;
        PlayerController.Player.Inventory.OnNewSlot += CreateSlot;
    }

    public static void Use()
    {
        //Instance.hotbarItems[0].Action.Execute(new CalculatingSystem.Context() {Owner = PlayerController.Player, Target = PlayerController.Player, Position = PlayerController.Player.transform.position});
        //if (Instance.hotbarItems[0].SingleUse) PlayerController.Player.Inventory.RemoveItem(Instance.hotbarItems[0], 1);
    }

    private void UpdateUI(int index)
    {
        if (!hotbarSlots.ContainsKey(index)) return;
        if (hotbarSlots[index].itemStack.Count == 0)
        {
            Destroy(hotbarSlots[index].gameObject);
            hotbarItems.Remove(hotbarSlots[index].itemStack.Item);
            hotbarSlots.Remove(index);
            return;
        }
        hotbarSlots[index].UpdateUI();
    }

    private void CreateSlot(ItemStack stack, int index)
    {
        if (stack.Item.Action != null)
        {
            var slot = Instantiate(itemSlotPrefab, bar).GetComponent<ItemSlot>();
            hotbarSlots.Add(index, slot);
            hotbarItems.Add(stack.Item);
            hotbarItemSlots.Add(slot);
            hotbarSlots[index].SetItem(stack);
        }
    }
}