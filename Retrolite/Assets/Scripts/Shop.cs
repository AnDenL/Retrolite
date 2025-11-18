using UnityEngine;
using Creatures;
public class ShopItem : Interactable
{
    public int price;

    [SerializeField] private Sprite soldSprite;
    [SerializeField] private TextMesh priceText;

    private bool isBought;

    private void Start()
    {
        transform.GetChild(1).GetComponent<Collider2D>().enabled = false;
        priceText.text = price.ToString();
        PlayerController.Player.Resources.Get(ResourceType.Money).OnChanged += ChangeTextColor;

        ChangeTextColor(PlayerController.Player.Resources.Get(ResourceType.Money).Count);
    }

    private void OnEnable()
    {
        if (PlayerController.Instance != null)
            PlayerController.Player.Resources.Get(ResourceType.Money).OnChanged += ChangeTextColor;
    }

    private void OnDisable() => PlayerController.Player.Resources.Get(ResourceType.Money).OnChanged -= ChangeTextColor;

    private void ChangeTextColor(int value) => priceText.color = value < price ? Color.red : Color.white;

    public override void Interact(Creature creature)
    {
        if (isBought)
            return;

        if (creature.Resources.TrySpend(ResourceType.Money, price))
        {
            isBought = true;
            GetComponent<SpriteRenderer>().sprite = soldSprite;
            transform.GetChild(1).GetComponent<ArcAnim>().DropTo(creature.transform.position);
            priceText.gameObject.SetActive(false);
        }
    }
}
