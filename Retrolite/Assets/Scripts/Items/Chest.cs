using System;
using System.Collections;
using UnityEngine;
using Creatures;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Chest : Interactable
{
    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] private Reward reward;

    private bool isOpen = false;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    } 

    public override void Interact(Creature creature)
    {
        if (isOpen)
        {
            animator.SetBool("IsOpen", false);
            isOpen = false;
            return;
        }

        isOpen = true;
        animator.SetBool("IsOpen", true);
        if (reward.Resources != null)
        {
            foreach (var res in reward.Resources)
            {
                ParticleManager.PlayParticle(res.Type, transform.position, creature.transform, res.Amount);
                creature.Resources.Add(res.Type, res.Amount);
            }
        }
        creature.HealthComponent.Heal(reward.Heal);
        creature.HealthComponent.AddMaximumHealth(reward.Health);

        if (reward.Items != null && reward.Items.Count > 0) StartCoroutine(SpawnObjects(reward.Items.ToArray(), creature.transform.position));
        reward = new Reward();
    }

    private IEnumerator SpawnObjects(GameObject[] items, Vector3 pos)
    {
        foreach (var item in items)
        {
            if (item.scene.IsValid() && item.scene.isLoaded)
            {
                item.SetActive(true);
                item.GetComponent<ArcAnim>()?.DropTo(pos);
                if (item.TryGetComponent(out Collider2D coll)) coll.enabled = false;
            }
            else Instantiate(item, transform.position, Quaternion.identity).GetComponent<ArcAnim>()?.DropTo(pos);
            yield return _waitForSeconds0_5;
        }
    }

    public void SetReward(Reward newReward)
    {
        reward = newReward;
    }
}

[Serializable]
public struct Reward
{   
    public float Heal;
    public float Health;
    public List<GameObject> Items;
    public List<ResourceReward> Resources;

    public static Reward Empty() => new(0,0,new(),new());

    public Reward(float heal = 0, float health = 0, List<GameObject> items = null, List<ResourceReward> resources = null)
    {
        Heal = heal;
        Health = health;
        Items = items;
        Resources = resources;
    }
}

[Serializable]
public struct ResourceReward
{
    public ResourceType Type;
    public int Amount;
}
