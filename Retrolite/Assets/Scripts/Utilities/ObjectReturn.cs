using System.Collections.Generic;
using UnityEngine;

public class ObjectReturn : MonoBehaviour
{
    public GameObject Prefab;
    public List<Transform> Targets;

    private float time;
    private void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out Rigidbody2D n)) Targets.Add(t);
        }
    }

    private void Update()
    {
        if (time > Time.time) return;

        foreach (var t in Targets)
        {
            if (t.localPosition.sqrMagnitude > 400)
            {
                Return(t);
                Targets.Remove(t);
                break;
            }
        }
        time += 1;
    }

    private void Return(Transform target)
    {
        Targets.Remove(target);
        Instantiate(Prefab, transform)
        .GetComponent<Hook>()
        .Set(target, (target.position - transform.position).normalized * 5 + transform.position, this);
    }
}
