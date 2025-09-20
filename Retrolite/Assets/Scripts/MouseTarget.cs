using UnityEngine;

public class MouseTarget : Creature
{
    public static MouseTarget instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    private void Update() => transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
}
