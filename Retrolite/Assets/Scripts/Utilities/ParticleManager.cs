using UnityEngine;
using Creatures;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public ParticleSystem[] ParticleSystems = new ParticleSystem[0];
    public ParticleSystem[] ResourceParticles = new ParticleSystem[0];

    private void Awake()
    {
        Instance = this;
    }

    public static void PlayParticle(int index, Vector2 position)
    {
        ParticleSystem ps = Instance.ParticleSystems[index];
        if (ps == null)
        {
            Debug.LogError($"Particle system at index {index} not found.");
            return;
        }

        ps.transform.position = position;
        ps.Play();
    }

    public static void PlayParticle(ResourceType index, Vector2 from, Transform to, int amount)
    {
        ParticleSystem ps = Instance.ResourceParticles[(int)index];
        if (ps == null)
        {
            Debug.LogError($"Particle system at index {index} not found.");
            return;
        }

        ps.transform.parent = to;
        ps.transform.localPosition = Vector3.zero;
        var emission = ps.emission;
        var shape = ps.shape;
        shape.position = (Vector3)from - to.position;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)amount));
        ps.Play();
    }
}
