using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

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

    private Coroutine reloadRoutine;

    public event Action OnReloadStart;
    public event Action<float> OnReload;
    public event Action OnReloadEnd;

    public bool IsReloading;
    public void Init(Creature owner)
    {
        this.owner = owner;
        handController = owner.GetComponentInChildren<LinePoints>();
        if (handController != null)
        {
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
        selected = guns.Count - 1;
        SelectGun(guns.Count - 1);
    }

    public void Scroll(int direction)  
    {
        if (guns.Count <= 1 || !owner.Controller.IsPlayer || Minimap.Instance.IsOpened) return;
        int previousSelected = selected;
        selected += direction;

        if (selected < 0) selected = guns.Count - 1;
        else if (selected > guns.Count - 1) selected = 0;
        if (previousSelected != selected) SelectGun(selected);
    }

    private void SelectGun(int index)
    {
        StopReloading();
        GunData selectedGun = guns[index];
        gun.Initialize(selectedGun, owner, this);
        ToggleHands(selectedGun.GunType == GunType.Empty);
        OnSelected?.Invoke(index);
    }

    public GunData Get() => guns[selected];

    public void Shoot() => gun.Fire();
    public void Reload()
    {
        if (!IsReloading) StartCoroutine(ReloadCoroutine());
    }

    public void StopReloading() 
    {
        if(reloadRoutine != null) StopCoroutine(reloadRoutine);
    }

    private IEnumerator ReloadCoroutine()
    {
        OnReloadStart?.Invoke();
        IsReloading = true;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / gun.Data.ReloadTime;
            OnReload?.Invoke(t);
            yield return null;
        }

        gun.Reload();
        IsReloading = false;
        OnReloadEnd?.Invoke();
    }
    public bool CanShoot() => gun.CanShoot();
    
    public void Rotate(Vector3 position)
    {
        Vector2 rawdirection = position - transform.position;

        rawdirection.Normalize();

        Vector2 direction = rawdirection.x < 0 ? -rawdirection : rawdirection;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        handTransform.localPosition = new Vector3(0.7f - Mathf.Abs(direction.y) / 10, 0f, rawdirection.y + 0.65f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}