using UnityEngine;

public class ZPosStart : MonoBehaviour
{
    private void Start() => transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
}
