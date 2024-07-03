using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _slots;
    [SerializeField] private GameObject _slot;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Button _active;
    [SerializeField] private Button _drop;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private InventoryItem _weaponItem;
    [SerializeField] private InventoryItem _activeItem;

    public List<InventoryItem> Items;

    public int? SelectedItem = null;
    public ItemBase Empty;

    private void Awake()
    {
        Game.Inventory = this;
        Items = new List<InventoryItem>();
        Items.Add(_weaponItem.CreateItem(Empty, 0, this));
        Items.Add(_activeItem.CreateItem(Empty, 1, this));
        while(_slots > Items.Count)
        {
            Items.Add(Instantiate(_slot, _spawnPoint).GetComponent<InventoryItem>().CreateItem(Empty, Items.Count, this));
        }
    }

    private void Update()
    {
        if(Game.Paused || Game.Player.IsRunning) return;

        if(_activeItem != Empty && Input.GetKeyDown(KeyCode.Space))
            _activeItem.Action();
        
        if(_weaponItem == Empty) return;
        if(Input.GetAxisRaw("Fire") != 0)
        {
            _weaponItem.Action();
        }
        if(Input.GetButtonDown("Reload")) Game.Weapon.ReloadWeapon();
    }

    public void ShowData(ItemBase item)
    {
        _name.text = item.Name;
        _description.text = item.Description;
        _active.interactable = item.CanAction;      
        _drop.interactable = item.CanDrop;
    }

    public void ShowData()
    {
        if(SelectedItem == null)
        {
            _name.text = " ";
            _description.text = " ";
            _active.interactable = false;      
            _drop.interactable = false;
        }
        else 
        {
            int i = SelectedItem.Value;
            _name.text = Items[i].Data.Name;
            _description.text = Items[i].Data.Description;
            _active.interactable = Items[i].Data.CanAction;      
            _drop.interactable = Items[i].Data.CanDrop;
        }
    }

    public void Active()
    {
        Items[SelectedItem.Value].Action();
        SelectedItem = null;
        ShowData();
    }

    public void Drop()
    {
        Items[SelectedItem.Value].Drop();
        SelectedItem = null;
        ShowData();
    }

    public void ReplaceItem(int id)
    {
        ItemBase item0 = Items[id].Data;
        ItemBase item1 = Items[SelectedItem.Value].Data;

        Items[SelectedItem.Value].ChangeItem(item0);
        Items[id].ChangeItem(item1);
    }

    public void NewItem(ItemBase item)
    {
        int i;
        if(item is WeaponItem) i = 0;
        else i = 2;
        while(Items[i].Data != Empty || i == 1)
        {
            i++;
            if(i > Items.Count - 1) return;
        }
        Items[i].ChangeItem(item);
    }

    public void DeleteItem(int id)
    {
        Items[id].ChangeItem(Empty);
    }

    public InventoryItem GetSelectedItem()
    {
        return Items[SelectedItem.Value];
    }
}