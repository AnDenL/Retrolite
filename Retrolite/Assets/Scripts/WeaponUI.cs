using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public Image WeaponImage;
    public AmmoUI AmmoUI;

    private List<GunData> guns = new List<GunData>();
    private int selected = 0;

    public void AddGun(GunData gun)
    {
        guns.Add(gun);
        selected = guns.Count - 1;
        UpdateUI();
    }

    private void Scroll(int direction)
    {
        int previousSelected = selected;
        selected += direction;

        if (selected < 0) selected = guns.Count - 1;
        else if (selected > guns.Count - 1) selected = 0;
        if (previousSelected != selected) UpdateUI();
    }

    private void UpdateUI()
    {
        WeaponImage.sprite = guns[selected].GunSprite;
        AmmoUI.SetAmmoTexture(guns[selected].BulletSprite);
        AmmoUI.SetAmmo(guns[selected].CurrentAmmo, guns[selected].MagazineSize);

        Hints.Show("Selected weapon: " + guns[selected].Name, 2f, AnimationCurve.Linear(0, 1, 1, 0));
    }
}
