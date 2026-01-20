using System;
using System.Collections;
using UnityEngine;
using Creatures;

public class Chest : Interactable
{
    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] private Reward reward;

    private bool isOpen = false;
    private Animator animator;

    private void Start() => animator = GetComponent<Animator>();

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

        if (reward.Items != null && reward.Items.Length > 0) StartCoroutine(SpawnObjects(reward.Items, creature.transform.position));
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
            }
            else Instantiate(item, transform.position, Quaternion.identity).GetComponent<ArcAnim>()?.DropTo(pos);
            yield return _waitForSeconds0_5;
        }
    }

    private void SetReward(Reward newReward)
    {
        reward = newReward;
    }
}

[Serializable]
public struct Reward
{
    [Serializable]
    public struct ResourceReward
    {
        public ResourceType Type;
        public int Amount;
    }
    
    public float Heal;
    public float Health;
    public GameObject[] Items;
    public ResourceReward[] Resources;

    public Reward(float heal = 0, float health = 0, GameObject[] items = null, ResourceReward[] resources = null)
    {
        Heal = heal;
        Health = health;
        Items = items;
        Resources = resources;
    }
}
