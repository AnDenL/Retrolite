using System.Collections.Generic;
using UnityEngine;
 
public class ListSO<T> : ScriptableObject
{
    [SerializeField] protected T[] entries;

    public T this[int index] => entries[index];

    public T GetRandom() => entries[Random.Range(0, entries.Length)];

    public T GetRandom(GameRandom rnd) => entries[rnd.Range(0, entries.Length)];
}