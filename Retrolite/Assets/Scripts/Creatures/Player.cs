using System;
using UnityEngine;


public class Player : Creature
{
    [Header("Player")]
    [SerializeField] Transform rotation;
    [SerializeField] Transform hand;
    [SerializeField] LinePoints arm1, arm2;
    [SerializeField] GameObject handsWithoutGun;
    [SerializeField] GameObject slashEffect;
    [SerializeField] GunBase gun;
    [SerializeField] TrailRenderer attackTrail;

    private Transform hand1, hand2;

    [Header("Interact")]
    [SerializeField] int money;
    [SerializeField] int bits;
    [SerializeField] ParticleSystem coinParticles, codeParticles;

    private GameObject lastInteractedObject;
    private Camera mainCamera;
    private LayerMask interactMask;

    private ParticleSystem.ShapeModule coinShape, codeShape;
    private ParticleSystem.EmissionModule coinEmission, codeEmission;

    public event Action<int> OnMoneyChange;
    public event Action<int> OnBitsChange;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        interactMask = LayerMask.GetMask("Interactable");

        hand1 = handsWithoutGun.transform.GetChild(0);
        hand2 = handsWithoutGun.transform.GetChild(1);

        coinShape = coinParticles.shape;
        coinEmission = coinParticles.emission;

        codeShape = codeParticles.shape;
        codeEmission = codeParticles.emission;
    }

    protected override void Update()
    {
        base.Update();
        Rotate();

        InteractObject();
        OutlineObject();
    }

    #region Gun

    public GunData SetGun(GunData gunData)
    {
        GunData previousGunData = gun.Data;
        gun.Set(gunData);
        if (gunData.GunType == GunType.Empty)
        {
            rotation.gameObject.SetActive(false);
            handsWithoutGun.SetActive(true);
            arm1.points[1] = hand1;
            arm2.points[1] = hand2;
        }
        else
        {
            rotation.gameObject.SetActive(true);
            handsWithoutGun.SetActive(false);
            arm1.points[1] = hand;
            arm2.points[1] = hand;
        }
        //gunUI.UpdateGunUI(gun);

        return previousGunData;
    }

    private void Rotate()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)transform.position + Vector2.down;
        direction.Normalize();

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            direction = -direction;
            Animator.SetBool("IsBackwards", Input.GetAxisRaw("Horizontal") > 0);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Animator.SetBool("IsBackwards", Input.GetAxisRaw("Horizontal") < 0);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        hand.localPosition = new Vector3(0.65f - Mathf.Abs(direction.y) / 6, 0f, direction.y);
        rotation.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    #endregion
    #region Interact

    private void InteractObject()
    {
        var temp = Physics2D.OverlapCircleAll(transform.position, 1.5f, interactMask);

        Collider2D nearestCollider = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in temp)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = collider;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestCollider != null)
        {
            nearestCollider.GetComponent<Interactable>()?.Interact(this);
        }
    }

    private void OutlineObject()
    {
        var temp = Physics2D.OverlapCircleAll(transform.position, 1.5f, interactMask);

        Collider2D nearestCollider = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in temp)
        {
            if (collider.CompareTag("Interactable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestCollider = collider;
                    nearestDistance = distance;
                }
            }
        }

        if (nearestCollider != lastInteractedObject)
        {
            if (lastInteractedObject != null) lastInteractedObject.GetComponent<Interactable>().CancelOutline();
            if (nearestCollider != null) lastInteractedObject = nearestCollider.gameObject;
            else lastInteractedObject = null;
        }

        if (nearestCollider != null)
            nearestCollider.GetComponent<Interactable>().Outline();
    }

    #endregion
    #region Money & Bits

    public bool Buy(int value)
    {
        if (money >= value)
        {
            money -= value;
            OnMoneyChange?.Invoke(money);
            return true;
        }
        return false;
    }

    public void AddMoney(int value) => AddMoney(value, transform.position);

    public void AddMoney(int value, Vector3 spawnPosition)
    {
        money += value;
        OnMoneyChange?.Invoke(money);

        coinShape.position = transform.InverseTransformPoint(spawnPosition);
        coinEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)value));

        coinParticles.Play();
    }

    public void AddCode(int value) => AddCode(value, transform.position);

    public void AddCode(int value, Vector3 spawnPosition)
    {
        bits += value;
        OnBitsChange?.Invoke(value);

        codeShape.position = transform.InverseTransformPoint(spawnPosition);
        codeEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)value));

        codeParticles.Play();
    }

    public float GetMoney() => money / 100;
    #endregion

    public void SetSaveData(SaveData data)
    {
        HealthComponent.SetHealth(data.PlayerHealth, data.PlayerMaxHealth);
        SetGun(data.PlayerWeapon);
        money = data.PlayerMoney;
        OnMoneyChange?.Invoke(money);
        bits = data.PlayerCode;
        OnBitsChange?.Invoke(bits);
    }
}