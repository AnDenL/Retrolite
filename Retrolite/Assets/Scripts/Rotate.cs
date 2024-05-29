using UnityEngine;

public class Rotate : MonoBehaviour
{
    Camera main;
    private void Start()
    {
        main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    void Update()
    {
        Vector3 diference = main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        // Vector3 diference = Gamepad();
        float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        Vector3 LocalScale = Vector3.one;

        if(rotateZ > 90 || rotateZ < -90){
            LocalScale.y = -1f;
        }else  {
            LocalScale.y = 1f;
        }

        transform.localScale = LocalScale;
    }

    Vector3 Gamepad()
    {
        Vector3 joy = Vector3.zero;
        
        joy.x = Input.GetAxis("JoyX");
        joy.y = Input.GetAxis("JoyY");

        return joy;
    }
}