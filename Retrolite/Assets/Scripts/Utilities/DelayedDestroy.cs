using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    [SerializeField] float delayTime = 2f;
    private void Start() => Destroy(gameObject, delayTime);
}
