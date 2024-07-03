using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;

    private List<HealthBase> _enemyHealth = new List<HealthBase>();

    public Tilemap Doors;
    public Tilemap Trigger;


    private void Start()
    {
        
    }

    private void StartBattle()
    {

    }

    public void AddEnemy(float x, float y)
    {
        GameObject obj = Instantiate(_enemyPrefab[Random.Range(0,_enemyPrefab.Length - 1)],transform);
        obj.transform.position = new Vector3(x,y,0);
        HealthBase hp = obj.GetComponent<HealthBase>();
        AddEnemy(hp);
    }

    public void AddEnemy(HealthBase health)
    {
        _enemyHealth.Add(health);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(CompareTag(other.tag)) StartBattle();
    }
}
