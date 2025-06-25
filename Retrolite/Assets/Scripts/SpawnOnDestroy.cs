using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded) Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
