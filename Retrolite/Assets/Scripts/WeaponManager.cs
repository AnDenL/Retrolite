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


    private void Init(Creature owner)
    {
        this.owner = owner;
        handController = owner.transform.Find("Arms").GetComponent<LinePoints>();
        handsWithoutGun = handController.GetEndPointPositions();
    }

    private void ToggleHands(bool active)
    {
        if (active)
        {
            handController.gameObject.SetActive(true);
            handController.SetEndPointPositions(handsWithoutGun);
        }
        else
        {
            handController.gameObject.SetActive(false);
            handController.SetEndPointPositions(handTransform);
        }
    }

    private void Update()
    {
        Rotate();
    }

    public void AddGun(GunData gunData)
    {
        guns.Add(gunData);
    }

    public void SelectGun(int index)
    {
        GunData selectedGun = guns[index];
        gun.Set(selectedGun);
        if (selectedGun.GunType == GunType.Empty)
        {
            ToggleHands(true);
        }
        else
        {
            ToggleHands(false);
        }
    }

    public void Shoot(Vector3 direction)
    {
        // Implement shooting logic here
    }
    
    private void Rotate()
    {
        Vector2 mousePosition = Game.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position + Vector2.down;
        direction.Normalize();

        direction = direction.x < 0 ? -direction : direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        handTransform.localPosition = new Vector3(0.65f - Mathf.Abs(direction.y) / 6, 0f, direction.y);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}