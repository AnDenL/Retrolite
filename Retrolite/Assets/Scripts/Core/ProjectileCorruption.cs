using UnityEngine;

public class ProjectileCorruption : Corruptible
{
    private void Awake()
    {
        OnBecameVulnerable += Destroy;
    }

    public void Destroy() => Destroy(gameObject);
}