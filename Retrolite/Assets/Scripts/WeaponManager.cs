using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoBehaviour
{
    public BulletPool bulletPool;

    [SerializeField] private Transform handTransform;
    [SerializeField] private GunBase gun;
    public GunBase Gun => gun;

    private List<GunData> guns = new();
    public IReadOnlyList<GunData> Guns => guns;
    private Creature owner;
    private LinePoints handController;
    private Transform[] handsWithoutGun;

    private int selected = 0;

    public Action<int> OnSelected;

    public void Init(Creature owner)
    {
        this.owner = owner;
        var arms = owner.transform.Find("Arms");
        if (arms != null)
        {
            handController = arms.GetComponent<LinePoints>();
            handsWithoutGun = handController.GetEndPointPositions();
        }
        guns = new List<GunData> { new() };
        SelectGun(0);
    }

    private void ToggleHands(bool active)
    {
        if (handController == null) return;
        if (active)
        {
            handController.transform.GetChild(0).gameObject.SetActive(true);
            handController.SetEndPointPositions(handsWithoutGun);
            handTransform.gameObject.SetActive(false);
        }
        else
        {
            handController.transform.GetChild(0).gameObject.SetActive(false);
            handController.SetEndPointPositions(handTransform);
            handTransform.gameObject.SetActive(true);
        }
    }

    public void AddGun(GunData gunData)
    {
        if (gunData.GunType == GunType.Empty) return;
        guns.Add(gunData);
        SelectGun(guns.Count - 1);
    }

    public void Scroll(int direction)  
    {
        if (guns.Count <= 1 || !owner.Controller.IsPlayer) return;
        int previousSelected = selected;
        selected += direction;

        if (selected < 0) selected = guns.Count - 1;
        else if (selected > guns.Count - 1) selected = 0;
        if (previousSelected != selected) SelectGun(selected);
    }

    private void SelectGun(int index)
    {
        GunData selectedGun = guns[index];
        gun.Initialize(selectedGun, owner);
        ToggleHands(selectedGun.GunType == GunType.Empty);
        OnSelected?.Invoke(index);
    }

    public GunData Get() => guns[selected];

    public void Shoot() => gun.Fire();
    public void Reload()
    {
        if (gun.isActiveAndEnabled) gun.Reload();
    }
    public bool CanShoot() => gun.CanShoot();
    
    public void Rotate(Vector3 position)
    {
        Vector2 direction = position - transform.position;

        direction.Normalize();

        direction = direction.x < 0 ? -direction : direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        handTransform.localPosition = new Vector3(0.7f - Mathf.Abs(direction.y) / 8, 0f, direction.y);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}