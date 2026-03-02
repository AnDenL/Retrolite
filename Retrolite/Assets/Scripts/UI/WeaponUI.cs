using UnityEngine.UI;
using UnityEngine;
using Creatures;

public class WeaponUI : MonoBehaviour
{
    public Image WeaponImage;
    public AmmoUI AmmoUI;

    [SerializeField] private GameObject panel;
    [SerializeField] private Image image;

    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = PlayerController.Instance.WeaponManager;
        weaponManager.OnSelected += UpdateUI;
        weaponManager.Gun.OnFire += UpdateUI;
        weaponManager.OnReloadStart += StartReload;
        weaponManager.OnReload += SetReload;
        weaponManager.OnReloadEnd += UpdateUI;
    }

    private void StartReload()
    {
        image.gameObject.SetActive(true);
    }

    private void SetReload(float fill)
    {
        image.fillAmount = 1 - fill;
    }

    private void UpdateUI(int selected)
    {
        image.gameObject.SetActive(false);
        GunData gun = weaponManager.Guns[selected];
        if (gun.GunType == GunType.Empty)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);

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
