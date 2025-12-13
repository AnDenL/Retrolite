using System;
using System.Collections.Generic;
using UnityEngine;

public class Corruptible : MonoBehaviour
{
    [SerializeField] private int maxStability = 1;
    public int MaxStability => maxStability;
    [SerializeField] private int stability;
    public int Stability => stability;

    [SerializeReference] public EditableParam[] editables;

    public bool IsCorrupted { get; private set; }

    public event Action OnBecameVulnerable;
    public event Action<int> OnCorrupting;

    private void Start()
    {
        stability = maxStability;
    }

    public void ApplyCorruption(int amount)
    {
        ParticleManager.PlayParticle("Glitch", transform.position);
        stability -= amount;
        OnCorrupting?.Invoke(stability);

        if (stability <= 0 && !IsCorrupted)
        {
            BecomeCorrupted();
        }
    }

    public void Redact() => CodeEditManager.Redact(gameObject.name.Replace("(Clone)", ""), transform.position, editables);

    public void BecomeCorrupted()
    {
        IsCorrupted = true;
        OnBecameVulnerable?.Invoke();
    }

    public void ResetStability()
    {
        stability = maxStability;
        IsCorrupted = false;
    }
}
