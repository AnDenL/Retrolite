using UnityEngine;

public class TrainingMode : MonoBehaviour
{
    public int enemyCountMin = 3;
    public int enemyCountMax = 6;
    public GameObject[] enemyPrefabs;

    public GameObject chestPrefab;
    public GameObject shopPrefab;
    public GameObject exitPrefab;

    public BoxCollider2D spawnArea;

    private int aliveEnemies;

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        int level = SaveSystem.CurrentSave.Level;
        int enemyCount = Random.Range(enemyCountMin, enemyCountMax + level / 3);

        aliveEnemies = enemyCount;
        for (int i = 0; i < enemyCount; i++)
            SpawnRandom(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]).GetComponent<HealthBase>().OnDeath += OnEnemyKilled;

        SpawnRandom(shopPrefab);
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
            SpawnRewardAndExit();
    }

    private void SpawnRewardAndExit()
    {
        SpawnRandom(chestPrefab);
        SpawnRandom(exitPrefab);
    }

    private GameObject SpawnRandom(GameObject prefab)
    {
        var pos = GetRandomPositionInArea();
        return Instantiate(prefab, pos, Quaternion.identity);
    }

    private Vector2 GetRandomPositionInArea()
    {
        Bounds b = spawnArea.bounds;
        return new Vector2(
            Random.Range(b.min.x, b.max.x),
            Random.Range(b.min.y, b.max.y)
        );
    }
}
