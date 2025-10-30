using UnityEngine;

public class PooledParticle : MonoBehaviour
{
    private ParticleSystem ps;
    private void Awake() => ps = GetComponent<ParticleSystem>();
    private void OnParticleSystemStopped() => gameObject.SetActive(false);
}