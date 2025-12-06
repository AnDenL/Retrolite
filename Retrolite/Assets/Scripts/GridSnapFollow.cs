using UnityEngine;

public class GridSnapFollow : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private Transform target;

    private void Update() =>
        transform.position = new Vector3(
            Mathf.Round(target.position.x / size) * size, 
            Mathf.Round(target.position.y / size) * size, 
            transform.position.z);
}