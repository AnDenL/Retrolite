using UnityEngine;

public class MaterialSetSeed : MonoBehaviour
{
    [SerializeField] private LevelGenerationBase generator;
    [SerializeField] private Material material;

    private void Start() => material.SetFloat("_Seed", (float)generator.seed / uint.MaxValue);
}