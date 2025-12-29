using System;
using UnityEngine;

public class CorruptibleBase : MonoBehaviour, ICorruptible
{
    [SerializeField] protected int maxStability = 1;
    public int MaxStability => maxStability;
    [SerializeField] protected int stability;
    public int Stability => stability;

    [SerializeReference] public EditableParam[] editables;

    public bool IsCorrupted { get; protected set; }

    public event Action OnBecameVulnerable;
    public event Action<int> OnCorrupting;

    protected void Start()
    {
        stability = maxStability;
    }

    public void ApplyCorruption(int amount, Creature source)
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
    public bool Break() { return false; }

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

    public virtual void Knockback(Vector2 dir, float strength) {}
}
