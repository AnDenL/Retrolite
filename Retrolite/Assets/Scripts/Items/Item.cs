using CalculatingSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Items/Empty")]
public class Item : ScriptableObject
{
    public string ItemName = "New Item";
    public string CustomDescription;
    public int MaxStack = 20;
    public bool Stackable => MaxStack > 1;
    public int Id => ItemName.GetHashCode();
    public Sprite Icon = null;
    public bool SingleUse;
    public AudioClip Sound;
    public GameAction Action;

    public void Activate(Context context) => Action.Execute(context);
}