using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{
    public float scale;
    public Transform chain;
    private Transform Player;
    private Vector3 pos;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        pos = chain.position - Player.position;
        transform.position = chain.position + (pos * scale);
    }
}
