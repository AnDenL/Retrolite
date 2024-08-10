using UnityEngine;

public class ZPositionUpdate : MonoBehaviour
{
    private void Update() => transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
}
