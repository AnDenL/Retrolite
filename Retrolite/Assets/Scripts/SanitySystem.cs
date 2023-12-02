using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanitySystem : MonoBehaviour
{
    private Health health;
    private float MaxSanity;
    private float percent;
    public float Sanity;
    public float stress;
    public Volume volume;
    public ParticleSystem particles;
    void Start()
    {
        health = GetComponent<Health>();
        MaxSanity = Sanity;
        StartCoroutine(LoseSanity());
    }
    IEnumerator LoseSanity()
    {
        var emission = particles.emission;
        while(Sanity > 0)
        {
            Sanity -= stress;
            float NewValue = (Sanity - MaxSanity) *-1;
            percent = NewValue / MaxSanity;
            volume.weight = percent;
            if (percent > 0.5f) {
                emission.rateOverTime = new ParticleSystem.MinMaxCurve((percent - 0.5f) * 4);
            }
            yield return null;
        }
        health.SetHealth(health.healthPoint);
    }
}
