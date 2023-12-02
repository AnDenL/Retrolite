using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform copyObject;
    void Update()
    {
        transform.rotation = copyObject.rotation;
        transform.localScale = copyObject.localScale;
    }
}
