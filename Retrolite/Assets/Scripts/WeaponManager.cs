using UnityEngine;
using System;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public BulletPool bulletPool;

    [SerializeField] private Transform handTransform;
    [SerializeField] private GunBase gun;

    private List<GunData> guns = new();
    private Creature owner;
    private LinePoints handController;
    private Transform[] handsWithoutGun;

    private int selected = 0;

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

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            int direction = Input.mouseScrollDelta.y > 0 ? -1 : 1;
            Scroll(direction);
        }
    }

    public void AddGun(GunData gunData)
    {
        if (gunData.GunType == GunType.Empty) return;
        guns.Add(gunData);
        SelectGun(guns.Count - 1);
    }

    private void Scroll(int direction)  
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
        gun.Set(selectedGun, owner);
        Hints.Show("Equipped " + selectedGun.Name, 0.5f, AnimationCurve.Linear(0, 1, 1, 0));
        ToggleHands(selectedGun.GunType == GunType.Empty);
    }

    public void Shoot()
    {
        gun.Fire();
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-90f, 90f));
    }

    public bool CanShoot()
    {
        return gun.Data.GunType != GunType.Empty && (gun.Data.fireTime <= Time.time || float.IsNaN(gun.Data.fireTime));
    }
    
    public void Rotate(Vector3 direction)
    {
        direction.Normalize();

        direction = direction.x < 0 ? -direction : direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        handTransform.localPosition = new Vector3(0.65f - Mathf.Abs(direction.y) / 6, 0f, direction.y);
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * 10f));
    }
}