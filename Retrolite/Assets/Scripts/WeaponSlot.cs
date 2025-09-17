using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    public Image GunImage;
    public TextMeshProUGUI AmmoCountText;
    public Transform BulletClip;
    public GameObject BulletPrefab;

    public void SetGun(GunData data)
    {
        GunImage.sprite = data.GunSprite;


    }
}