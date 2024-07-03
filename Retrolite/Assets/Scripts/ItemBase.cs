using UnityEngine;
[CreateAssetMenu(fileName = "ItemBase", menuName = "Items/Item", order = 1)]
public class ItemBase : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;

    [Header("Type")]

    public bool CanAction;
    public bool CanDrop;

    public virtual void Action(int id)
    {

    }
}
