using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] short targetWeight;
    [SerializeField] float zOffset;

    private void Start()
    {
        Game.mainCamera = Camera.main;
        Game.pixelCamera = Game.mainCamera.GetComponent<PixelPerfectCamera>();
    }

    private void Update()
    {
        if (Game.IsPaused && target != null) return;
        Vector3 mousePosition = (Game.mainCamera.ScreenToWorldPoint(Input.mousePosition) - target.position) / targetWeight;
        transform.position = target.position + mousePosition + Vector3.forward * zOffset;
    }
}
