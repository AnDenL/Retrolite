using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Items/Empty")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
}