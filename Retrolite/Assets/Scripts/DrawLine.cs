using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer line;
    public Transform boss;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(1, boss.position - transform.position);
    }
}
