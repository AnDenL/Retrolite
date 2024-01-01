using UnityEngine;

public class ZPosition : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
}
