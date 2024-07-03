using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemBase Data; 
    public int Id;
    public Image Frame;
    public Image Fill;
    private Image _image;
    private Inventory _inventory;
    private Color _selectedColor = new Color(1,1,1,0.2f);
    private Color _defaultColor = new Color(0,0,0,0.5f);

    public void Action()
    {
        Data.Action(Id);
        Frame.color = _defaultColor;
    }
    public void Drop()
    {
        Instantiate(Data.Prefab, Game.Player.transform.position, Quaternion.identity);
        _inventory.DeleteItem(Id);
        Frame.color = _defaultColor;
        _inventory.ShowData();
    }
    public InventoryItem CreateItem(ItemBase Data, int Id, Inventory _inventory)
    {
        this.Id = Id;
        this._inventory = _inventory;
        if(Data == _inventory.Empty) this.Data = Data;
        else this.Data = Instantiate(Data);

        GetComponent<Image>().sprite = Data.Icon;

        return this;
    }

    public void ChangeItem(ItemBase Data)
    {
        this.Data = Data;

        GetComponent<Image>().sprite = Data.Icon;
        if(Id == 0)
        {
            if(Data is WeaponItem item)
            {
                Game.Weapon.NewWeapon(item);
            }
        }
    }

    public void Click()
    {
        if(_inventory.SelectedItem == null)
        {
            if(Data != _inventory.Empty) 
            {
                _inventory.SelectedItem = Id;
                Frame.color = _selectedColor;
            }
        }
        else if(_inventory.SelectedItem  == Id)
        {
            _inventory.SelectedItem = null;
            Frame.color = _defaultColor;
        }
        else
        {
            if(CanReplaceItem()) _inventory.ReplaceItem(Id);
            _inventory.GetSelectedItem().Frame.color = _defaultColor;
            Frame.color = _defaultColor;
            _inventory.SelectedItem = null;
        }
        _inventory.ShowData();
    }

    public void ShowData()
    {
        if(_inventory.SelectedItem == null)_inventory.ShowData(Data);
    }
    private bool CanReplaceItem()
    {
        ItemBase selectedItem = _inventory.GetSelectedItem().Data;

        if (Id == 0)
        {
            return selectedItem is WeaponItem;
        }
        else if (Id == 1)
        {
            return selectedItem is ActiveItem;
        }
        else if (_inventory.SelectedItem == 0)
        {
            return Id >= 2 && (Data is WeaponItem || Data == _inventory.Empty);
        }
        else if (_inventory.SelectedItem == 1)
        {
            return Id >= 2 && (Data is ActiveItem || Data == _inventory.Empty);
        }

        return Id >= 2 && _inventory.SelectedItem >= 2;
    }
}
