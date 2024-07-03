using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _weigth;
    private Camera _camera;

    private void Start() => _camera = GetComponent<Camera>();
    private void Update()
    {
        Vector3 mouse = _camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = ((_target.position * (_weigth - 1)) + mouse) / _weigth; 
    }
}
