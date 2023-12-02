using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    public float delay;
    void Start()
    {
        Invoke("Destroy",delay);
    }
    
    void Destroy() => Destroy(gameObject);
}
