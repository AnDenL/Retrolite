using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] short targetWeight;
    [SerializeField] float zOffset;

    private void Start()
    {
        Game.mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Menu.IsPaused) return;
        Vector3 mousePosition = (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - target.position) / targetWeight;
        transform.position = target.position + mousePosition + Vector3.forward * zOffset;
    }
}
