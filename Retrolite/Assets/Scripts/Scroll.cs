using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public float modify;
    private float Y;

    void Start()
    {
        Y = transform.position.y;
    }

    public void Scrolling(float s)
    {
        transform.position = new Vector2(transform.position.x,Y - s * (modify * Screen.height));
    }
}
