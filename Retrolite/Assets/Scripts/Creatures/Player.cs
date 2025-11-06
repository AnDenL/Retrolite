using UnityEngine;

public class Player : Creature
{
    [SerializeField] private AudioClip step;
    private AudioSource source;

    private void Start() => source = GetComponent<AudioSource>();

    public void Step()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(step);
    }
}