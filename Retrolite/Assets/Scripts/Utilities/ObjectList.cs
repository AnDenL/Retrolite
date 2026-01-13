using UnityEngine;

[CreateAssetMenu(fileName = "ObjectList", menuName = "Game/Utilities/ObjectList")]
public class ObjectList : ScriptableObject
{
    public GameObject[] Entries;

    public GameObject GetRandom() => Entries[Random.Range(0, Entries.Length)];
}
