using UnityEngine;

public class Scroll : MonoBehaviour
{
    private float Y;
    public float modify = 0f;

    void Start()
    {
        Y = transform.position.y;
    }

    public void Scrolling(float s)
    {
        transform.position = new Vector2(transform.position.x,Y - s * (modify * Screen.height));
    }
}