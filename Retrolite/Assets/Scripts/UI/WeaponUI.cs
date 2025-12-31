using UnityEngine.UI;
using UnityEngine;
using Creatures;

public class WeaponUI : MonoBehaviour
{
    public Image WeaponImage;
    public AmmoUI AmmoUI;

    [SerializeField] private GameObject Panel;

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
        GunData gun = weaponManager.Guns[selected];
        if (gun.GunType == GunType.Empty)
        {
            Panel.SetActive(false);
            return;
        }

        Panel.SetActive(true);

        WeaponImage.sprite = gun.GunSprite;
        AmmoUI.SetAmmoTexture(gun.BulletData.BulletSprite);
        AmmoUI.SetAmmo(gun.CurrentAmmo, gun.MagazineSize);

        Hints.Show("Equipped " + weaponManager.Get().Name, 0.5f, AnimationCurve.Linear(0, 1, 1, 0));
    }

    private void UpdateUI()
    {
        AmmoUI.SetAmmo(weaponManager.Get().CurrentAmmo, weaponManager.Get().MagazineSize);
    }
}
