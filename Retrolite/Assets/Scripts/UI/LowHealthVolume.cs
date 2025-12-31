using UnityEngine;
using UnityEngine.Rendering;

public class LowHealthVolume : MonoBehaviour
{
    public Volume volume;
    public float intensity;

    private void Awake() => volume = GetComponent<Volume>();

    private void Update()
    {
        volume.weight = intensity + Mathf.Sin(Time.time * (10 * intensity)) * 0.2f;
    }
}
