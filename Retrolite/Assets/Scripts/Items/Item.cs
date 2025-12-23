using CalculatingSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Items/Empty")]
public class Item : ScriptableObject
{
    public string ItemName = "New Item";
    public Sprite Icon = null;
    [SerializeReference] public ActionNode Action;

    public void Activate(FormulaContext context) => Action.Execute(context);
}