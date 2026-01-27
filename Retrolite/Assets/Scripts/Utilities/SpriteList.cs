using UnityEngine;

[CreateAssetMenu(fileName = "SpriteList", menuName = "Game/Utilities/SpriteList")]
public class SpriteList : ScriptableObject
{
    public Sprite[] Entries;

    public Sprite RandomSprite() => Entries[Random.Range(0, Entries.Length)];
    public Sprite RandomSprite(GameRandom rnd) => Entries[rnd.Range(0, Entries.Length)];
}
