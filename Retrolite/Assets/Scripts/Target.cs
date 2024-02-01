using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform hand;
    private Camera main;
    private void Start()
    {
        main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    private void Update()
    {
        Vector2 playerPos = hand.position * 6;
        Vector2 mousePos = (main.ScreenToWorldPoint(Input.mousePosition));
        transform.position = new Vector2((playerPos.x + (mousePos.x * 0.2f)) / (6 + 0.2f),(playerPos.y + (mousePos.y * 0.2f)) / (6 + 0.2f));
    }
}