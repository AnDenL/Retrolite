using UnityEngine;

public class MouseTarget : Creature
{
    public static MouseTarget instance;

    protected override void Awake()
    {
        instance = this;
    }
    protected override void Update() => transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
}
