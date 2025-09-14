using UnityEngine;

public class MouseTarget : Creature
{
    public static MouseTarget instance;

    private void Awake() => instance = this;
    private void Update() => transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
}