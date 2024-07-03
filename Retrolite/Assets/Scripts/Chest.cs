using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Trigger
{
    [SerializeField] private GameObject[] _loot;
    [SerializeField] private int _minLoot;
    [SerializeField] private int _maxLoot;
    [SerializeField] private int _minMoney;
    [SerializeField] private int _maxMoney;

    private bool _isOpened = false;

    public override void OnEnter()
    {

    }

    public override void Activate()
    {
        if(_isOpened) return;
        Game.Player.AddMoney(Random.Range(_minMoney,_maxMoney));

        int r = Random.Range(_minLoot, _maxLoot);

        for(int i = 0; i < r; i++)
        {
            StartCoroutine(ItemDrop(Instantiate(_loot[Random.Range(0, _loot.Length)], transform).transform));
        }
        _isOpened = true;
    }

    public override void OnExit()
    {
        
    }

    private IEnumerator ItemDrop(Transform item)
    {
        float t = 0;
        Vector3 direction = Random.insideUnitCircle.normalized;

        while(t < 1)
        {
            item.position += direction * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
    }
}
