using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] short targetWeight;

    private void Update()
    {
        if (Menu.isPaused) return;
        Vector3 mousePosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position) / targetWeight;
        transform.position = target.position + mousePosition;
    }
}
