using System;
using UnityEngine;


public class Player : Creature
{
    [Header("Player")]
    [SerializeField] int money;
    [SerializeField] int bits;
    [SerializeField] ParticleSystem coinParticles, codeParticles;

    private GameObject lastInteractedObject;
    private LayerMask interactMask;

    private ParticleSystem.ShapeModule coinShape, codeShape;
    private ParticleSystem.EmissionModule coinEmission, codeEmission;

    public WeaponManager WeaponManager;

    public event Action<int> OnMoneyChange;
    public event Action<int> OnBitsChange;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        interactMask = LayerMask.GetMask("Interactable");

        coinShape = coinParticles.shape;
        coinEmission = coinParticles.emission;

        codeShape = codeParticles.shape;
        codeEmission = codeParticles.emission;
    }

    protected override void Update()
    {
        base.Update();

        OutlineObject();
        if (Input.GetKeyDown(KeyCode.E))
            InteractObject();
    }

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
        money = data.PlayerMoney;
        OnMoneyChange?.Invoke(money);
        bits = data.PlayerCode;
        OnBitsChange?.Invoke(bits);
    }
}