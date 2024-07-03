using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Items/Weapon", order = 1)]
public class WeaponItem : ItemBase
{
    public GameObject BulletPrefab;
    public float ReloadTime;
    public float Spread;
    public string Range;
    public string BulletSpeed;
    public string FireSpeed;
    public string Damage;
    public int Magazine;

    public override void Action(int id)
    {
        Game.Weapon.Fire();
    }
}
