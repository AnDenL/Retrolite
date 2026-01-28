using UnityEngine;

[CreateAssetMenu(menuName = "Game/Utilities/NameList")]
public class WeaponNameList : ScriptableObject
{
    public string[] Adjectives;
    public string[] Nouns;
    public string[] Suffixes;

    public string GetRandomName(GameRandom rnd)
    {
        string adj = rnd.Chance(0.5f) ? Adjectives[rnd.Range(0, Adjectives.Length)] + " " : "";
        string noun = Nouns[rnd.Range(0, Nouns.Length)];
        string suf = rnd.Chance(0.5f) ? " " + Suffixes[rnd.Range(0, Suffixes.Length)] : "";
        
        return $"{adj}{noun}{suf}";
    }
}