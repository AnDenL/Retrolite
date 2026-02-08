using System.Collections.Generic;
using System.Linq;
using Creatures;
using UnityEngine;

public class CreaturesManager : MonoBehaviour
{
    public static CreaturesManager Instance;


    public int CreaturePerFrame = 2;

    private readonly float activationDistSqr = 1225;
    private readonly Queue<Creature> creatureQueue= new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static void RegisterCreature(Creature creature)
    {
        Instance.creatureQueue.Enqueue(creature);
    }

    private void Update()
    {
        if (creatureQueue.Count == 0) return;

        int count = Mathf.Min(CreaturePerFrame, creatureQueue.Count);

        Vector2 playerPos = PlayerController.Player.transform.position;
        
        for (int i = 0; i < count; i++)
        {
            Creature creature = creatureQueue.Dequeue();
            
            if (creature != null)
            {
                UpdateEnemyState(creature, playerPos);
                creatureQueue.Enqueue(creature);
            }
        }
    }

    private void UpdateEnemyState(Creature creature, Vector2 playerPos)
    {
        float distSqr = ((Vector2)creature.transform.position - playerPos).sqrMagnitude;
        
        creature.IsActive = distSqr < activationDistSqr;
    }
}