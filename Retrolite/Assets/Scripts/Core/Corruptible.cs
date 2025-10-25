using System;
using UnityEngine;

public class Corruptible : MonoBehaviour
{
    [SerializeField] private int maxStability = 1;
    public int MaxStability => maxStability;
    [SerializeField] private int stability;
    public int Stability => stability;

    public bool isCorrupted { get; private set; }

    public event Action OnBecameVulnerable;
    public event Action<int> OnCorrupting;

    private void Start()
    {
        stability = maxStability;
    }

    public void ApplyCorruption(int amount)
    {
        ParticleManager.PlayParticle(7, transform.position);
        stability -= amount;
        OnCorrupting?.Invoke(stability);

        if (stability <= 0 && !isCorrupted)
        {
            BecomeCorrupted();
        }
    }

    public void BecomeCorrupted()
    {
        isCorrupted = true;
        OnBecameVulnerable?.Invoke();
    }

    public void ResetStability()
    {
        stability = maxStability;
        isCorrupted = false;
    }
}
