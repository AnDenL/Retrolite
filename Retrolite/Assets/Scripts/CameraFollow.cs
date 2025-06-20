using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private short targetWeight;

    private void Update()
    {
        if (Menu.isPaused) return;
        Vector3 mousePosition = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position) / targetWeight;
        transform.position = target.position + mousePosition;
    }
}
