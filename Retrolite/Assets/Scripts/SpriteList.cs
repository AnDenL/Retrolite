using UnityEngine;

[CreateAssetMenu(fileName = "SpriteList", menuName = "Game/SpriteList")]
public class SpriteList : ScriptableObject
{
    public Sprite[] Entries;

    public Sprite RandomSprite() => Entries[Random.Range(0, Entries.Length - 1)];
}
