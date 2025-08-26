using UnityEngine;

[CreateAssetMenu(fileName = "BulletRegistry", menuName = "Game/BulletRegistry")]
public class ObjectList : ScriptableObject
{
    public GameObject[] Entries;
}
