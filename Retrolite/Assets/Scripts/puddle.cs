using UnityEngine;

public class puddle : MonoBehaviour
{
    private ParticleSystem particles;
    private void Start() {
        particles = GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            particles.Play();
        }
    }
}
