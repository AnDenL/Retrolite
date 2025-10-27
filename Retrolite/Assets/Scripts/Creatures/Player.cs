using System;
using UnityEngine;


public class Player : Creature
{
    [Header("Player")]
    [SerializeField] int money;
    [SerializeField] int bits;
    [SerializeField] ParticleSystem coinParticles, codeParticles;

    private ParticleSystem.ShapeModule coinShape, codeShape;
    private ParticleSystem.EmissionModule coinEmission, codeEmission;

    public WeaponManager WeaponManager;

    public event Action<int> OnMoneyChange;
    public event Action<int> OnBitsChange;

    private void Start()
    {
        
    }

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