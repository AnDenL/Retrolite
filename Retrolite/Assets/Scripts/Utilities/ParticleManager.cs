using UnityEngine;
using Creatures;
using System.Linq;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    public static Dictionary<string, int> ParticleIndices = new();

    public ParticleSystem[] ParticleSystems = new ParticleSystem[0];
    public ParticleSystem[] ResourceParticles = new ParticleSystem[0];

    private ParticlePool[] resourceParticlePools;

    private void Awake()
    {
        Instance = this;

        ParticleIndices = ParticleSystems
            .Select((ps, index) => new { ps, index })
            .ToDictionary(x => x.ps.name, x => x.index);

        resourceParticlePools = ResourceParticles.Select(ps => new ParticlePool(ps, 5, transform)).ToArray();
    }

    public static void PlayParticle(string index, Vector2 position) => PlayParticle(ParticleIndices[index], position);

    public static void PlayParticle(int index, Vector2 position)
    {
        ParticleSystem ps = Instance.ParticleSystems[index];
        if (ps == null)
        {
            Debug.LogError($"Particle pool at index {index} not found.");
            return;
        }

        ps.transform.position = position;
        ps.gameObject.SetActive(true);
        ps.Play();

        ps.Play();
    }

    public static void PlayParticle(ResourceType index, Vector2 from, Transform to, int amount)
    {
        ParticlePool pool = Instance.resourceParticlePools[(int)index];
        if (pool == null)
        {
            Debug.LogError($"Resource particle pool at index {index} not found.");
            return;
        }

        pool.PlayParticle(from, to, amount);
    }
}

public class ParticlePool
{
    public ParticleSystem ParticlePrefab;

    private ParticleSystem[] pool;
    private int poolSize;

    public ParticlePool(ParticleSystem prefab, int size, Transform parent = null)
    {
        ParticlePrefab = prefab;
        poolSize = size;
        pool = new ParticleSystem[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Object.Instantiate(ParticlePrefab, parent);
            pool[i].gameObject.AddComponent<PooledParticle>();
            pool[i].gameObject.SetActive(false);
        }
    }

    public void PlayParticle(Vector2 position)
    {
        var ps = GetParticleSystem();

        if (ps != null)
        {
            ps.transform.position = position;
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        else
        {
            Debug.LogWarning("No available particle systems in the pool.");
        }
    }

    public void PlayParticle(Vector2 from, Transform to, int amount)
    {
        var ps = GetParticleSystem();

        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.transform.parent = to;
            ps.transform.localPosition = Vector3.zero;
            var emission = ps.emission;
            var shape = ps.shape;
            shape.position = (Vector3)from - to.position;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)amount));
            ps.gameObject.SetActive(true);
            ps.Play();
        }
        else
        {
            Debug.LogWarning("No available particle systems in the pool.");
        }
    }

    private ParticleSystem GetParticleSystem()
    {
        ParticleSystem ps = null;
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].IsAlive(true))
            {
                ps = pool[i];
                break;
            }
        }

        return ps;
    }
}