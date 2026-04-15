using UnityEngine;

public class MaterialSetSeed : MonoBehaviour
{
    [SerializeField] private LevelPlacer generator;
    [SerializeField] private Material material;

    private void Start() => material.SetFloat("_Seed", (float)generator.seed / uint.MaxValue);
}