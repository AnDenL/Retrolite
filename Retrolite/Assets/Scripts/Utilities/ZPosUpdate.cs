using UnityEngine;

public class ZPosUpdate : MonoBehaviour
{
    private void Update() => transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
}
