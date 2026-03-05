using System;
using UnityEngine;

public class CorruptibleBase : MonoBehaviour, ICorruptible
{
    [SerializeField] protected int maxStability = 1;
    public int MaxStability => maxStability;
    [SerializeField] protected int stability;
    public int Stability => stability;

    [SerializeReference] public EditableParam[] editables;

    [Range(1f,15f)]
    public float RecoveryTime = 5f;

    public bool IsCorrupted { get; set; }
    [HideInInspector] public bool IsBedingEdited;

    public event Action<bool> OnVulnerabilityChange;
    public event Action<int> OnCorrupting;

    protected void Start()
    {
        stability = maxStability;
    }

    public void ApplyCorruption(int amount, Creature source)
    {
        ParticleManager.PlayParticle("Glitch", transform.position, 5);
        stability -= amount;
        OnCorrupting?.Invoke(stability);

        if (stability <= 0 && !IsCorrupted)
        {
            BecomeCorrupted();
        }
    }

    public void Redact()
    {
        IsBedingEdited = true;
        CodeRedactSystem.Redact(gameObject.name.Replace("(Clone)", ""), transform.position, editables, this);
    }

    public bool Break() { return false; }

    public void BecomeCorrupted()
    {
        IsCorrupted = true;
        OnVulnerabilityChange?.Invoke(true);
        Invoke(nameof(ResetStability), RecoveryTime);
    }

    public void ResetStability()
    {
        if (IsBedingEdited) return;
        stability = maxStability;
        IsCorrupted = false;
        OnVulnerabilityChange?.Invoke(false);
    }

    public virtual void Knockback(Vector2 dir, float strength) {}
}
