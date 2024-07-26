using UnityEngine;

[CreateAssetMenu(fileName = "DefaultDungeon", menuName = "Dungeon", order = 0)]
public class Dungeon : ScriptableObject 
{
    public int Iterations = 10;
    public int WalkLength = 10;
    public bool StartRandomly = true;
}
