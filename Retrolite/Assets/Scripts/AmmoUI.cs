using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image bulletPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Settings")]
    [SerializeField] private int maxIcons = 40;

    private List<Image> pool = new();

    private void Awake()
    {
        for (int i = 0; i < maxIcons; i++)
        {
            Image bullet = Instantiate(bulletPrefab, container);
            bullet.gameObject.SetActive(false);
            pool.Add(bullet);
        }
    }

    public void SetAmmoTexture(Sprite sprite)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].sprite = sprite;
        }
    }

    public void SetAmmo(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
        ammoText.color = current > 0 ? Color.white : Color.red;
        int toShow = Mathf.Min(current, maxIcons);

        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].gameObject.SetActive(i < toShow);
        }
    }
}
