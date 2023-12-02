using UnityEngine;
using UnityEngine.UI;

public class NewObject : MonoBehaviour
{
    public GameObject createdObject;
    void Start()
    {
        CreateObject();
    }
    public void CreateObject()
    {
        Instantiate(createdObject, transform.parent.GetChild(0));
    }
}
