using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ActiveItem : ItemBase
{
    public bool Charged = true;
    public Image Fill;
    [SerializeField] private float _chargeTime = 0.5f;
    [SerializeField] private bool _singleUse;
    private void Awake()
    {
        Charged = true;
    }
    public override void Action(int id)
    {
        if(Charged)
        {
            Active();
            if(_singleUse) Game.Inventory.DeleteItem(id);
            else 
            {
                Fill = Game.Inventory.Items[id].Fill;
                Game.instance.StartCharge(_chargeTime, this);
            }
        }
    }

    protected virtual void Active()
    {

    }
}