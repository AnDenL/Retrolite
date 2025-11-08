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
    }

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
