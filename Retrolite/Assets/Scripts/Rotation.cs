using UnityEngine;

public class Rotate : MonoBehaviour
{
    Camera main;
    private void Start() => main = FindObjectOfType<Camera>();
    private void Update()
    {
        if(Game.Paused) return;
        
        Vector3 diference = main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        Vector3 LocalScale = Vector3.one;

        if(rotateZ > 90 || rotateZ < -90) LocalScale.y = 1f;

        else LocalScale.y = -1f;

        transform.localScale = LocalScale;
    }
}

