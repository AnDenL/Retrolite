using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject prefabToSpawn;

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded) Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
