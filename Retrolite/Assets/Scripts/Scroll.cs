using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    public float _moddify;
    private float Y;

    void Start()
    {
        Y = transform.position.y;
    }

    public void Scrolling(float s)
    {
        transform.position = new Vector2(transform.position.x,Y - s * (_moddify * Screen.height));
    }
}
