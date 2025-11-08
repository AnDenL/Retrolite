using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Creatures;

public class WeaponUI : MonoBehaviour
{
    public Image WeaponImage;
    public AmmoUI AmmoUI;

    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = PlayerController.Instance.WeaponManager;
        weaponManager.OnSelected += UpdateUI;
        weaponManager.Gun.OnFire += UpdateUI;
        weaponManager.Gun.OnReloadEnd += UpdateUI;
    }

    private void UpdateUI(int selected)
    {
        WeaponImage.sprite = weaponManager.Guns[selected].GunSprite;
        AmmoUI.SetAmmoTexture(weaponManager.Guns[selected].BulletSprite);
        AmmoUI.SetAmmo(weaponManager.Guns[selected].CurrentAmmo, weaponManager.Guns[selected].MagazineSize);

        Hints.Show("Equipped " + weaponManager.Get().Name, 0.5f, AnimationCurve.Linear(0, 1, 1, 0));
    }

    private void UpdateUI()
    {
        AmmoUI.SetAmmo(weaponManager.Get().CurrentAmmo, weaponManager.Get().MagazineSize);
    }
}
