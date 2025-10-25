using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerEvent : MonoBehaviour
{
    public event Action<Collider2D> OnTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggered?.Invoke(collision);
    }
}