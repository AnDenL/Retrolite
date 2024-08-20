using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanitySystem : MonoBehaviour
{
    private Health health;
    private float MaxSanity = 10000;
    private float percent;
    public float Sanity = 10000;
    public float stress;
    public Volume volume;
    public ParticleSystem particles;

    private void Awake()
    {
        Game.Sanity = this; 
    }
    private void Start()
    {
        health = GetComponent<Health>();
        StartCoroutine(LoseSanity());
    }

    public float SanityPercent()
    {
        return Sanity / MaxSanity;
    }

    IEnumerator LoseSanity()
    {
        var emission = particles.emission;
        while(Sanity > 0)
        {
            Sanity -= Time.deltaTime * stress;

            if(Sanity > MaxSanity) Sanity = MaxSanity;

            float NewValue = (Sanity - MaxSanity) *-1;
            percent = NewValue / MaxSanity;
            volume.weight = percent;
            if (percent > 0.5f) {
                emission.rateOverTime = new ParticleSystem.MinMaxCurve((percent - 0.5f) * 12);
            }
            yield return null;
        }
        health.SetHealth(health.healthPoint);
    }
}
