using System;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField]
    private Reward reward;

    private bool isOpen = false;
    private Animator animator;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        animator = GetComponent<Animator>();
    }

    public override void Interact(Player player)
    {
        if (isOpen) return;
        isOpen = true;
        animator.SetBool("IsOpen", true);
        player.AddMoney(reward.money);
        player.AddCode(reward.code);
        player.AddHealth(reward.health);
        // Spawn objects
        reward = new Reward();
    }
}

[Serializable]
public struct Reward
{
    public int money;
    public int code;
    public float health;
    public GameObject item;

    public Reward(int money = 0, int code = 0, float health = 0, GameObject item = null)
    {
        this.money = money;
        this.code = code;
        this.health = health;
        this.item = item;
    }
}
