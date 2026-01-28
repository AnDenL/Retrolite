using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSeedSetter : MonoBehaviour, IGenerationStruct
{
    public void Generate(GameRandom random)
    {
        var particle = GetComponent<ParticleSystem>();

        particle.Stop();
        particle.useAutoRandomSeed = false;
        particle.randomSeed = random.NextUInt();
        particle.Play();
    }
}
