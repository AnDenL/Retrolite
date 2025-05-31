using UnityEngine;
[CreateAssetMenu(fileName = "Potion", menuName = "ActiveItems/Potion", order = 1)]
public class Potion : ActiveItem
{
    [SerializeField] private float _healthChange;
    [SerializeField] private float _sanityChange;
    [SerializeField] private float _immortalityTime;
    [SerializeField] private float _extraLife;

    protected override void Active()
    {
        Game.Player.SetHealth(_healthChange);
        Game.Player.MaxHealth += _extraLife;
        Game.Player.Immortality(_immortalityTime);
        Game.Player.Sanity += _sanityChange;
    }
}
