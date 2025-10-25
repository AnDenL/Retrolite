using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    public ParticleSystem[] particleSystems = new ParticleSystem[0];

    private void Awake()
    {
        Instance = this;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public static void PlayParticle(int index, Vector2 position)
    {
        ParticleSystem ps = Instance.particleSystems[index];
        if (ps == null)
        {
            Debug.LogError($"Particle system at index {index} not found.");
            return;
        }

        ps.transform.position = position;
        ps.Play();
    }
}
