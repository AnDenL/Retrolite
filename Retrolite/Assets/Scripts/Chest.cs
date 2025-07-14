using System;
using System.Collections;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] Reward reward;

    private bool isOpen = false;
    private Animator animator;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        animator = GetComponent<Animator>();
    }

    public override void Interact(Player player)
    {
        if (isOpen)
        {
            animator.SetBool("IsOpen", false);
            isOpen = false;
            return;
        }
        isOpen = true;
        animator.SetBool("IsOpen", true);
        player.AddMoney(reward.money, transform.position);
        player.AddCode(reward.code, transform.position);
        player.AddHealth(reward.health);
        if (reward.items != null && reward.items.Length > 0) StartCoroutine(SpawnObjects(reward.items, player.transform.position));
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
            yield return new WaitForSeconds(0.5f);
        }
    }
}

[Serializable]
public struct Reward
{
    public int money;
    public int code;
    public float health;
    public GameObject[] items;

    public Reward(int money = 0, int code = 0, float health = 0, GameObject[] items = null)
    {
        this.money = money;
        this.code = code;
        this.health = health;
        this.items = items;
    }
}
