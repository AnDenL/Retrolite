using Creatures;
using UnityEngine;

public class WallMaterial : MonoBehaviour
{
    public Material material;

    public void Update()
    {
        material.SetVector("_Position", PlayerController.Player.transform.position + Vector3.up);
    }
}