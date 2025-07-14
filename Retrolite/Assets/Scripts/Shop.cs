using UnityEngine;
using System.Collections;

public class Shop : Interactable
{
    public int price;

    [SerializeField] Sprite soldSprite;
    [SerializeField] TextMesh priceText;

    private bool isBought;

    private void Start()
    {
        transform.GetChild(1).GetComponent<Collider2D>().enabled = false;
        priceText.text = price.ToString();
    }

    public override void Interact(Player player)
    {
        if (isBought)
            return;
        if (player.Buy(price))
        {
            isBought = true;
            GetComponent<SpriteRenderer>().sprite = soldSprite;
            transform.GetChild(1).GetComponent<ArcAnim>().DropTo(player.transform.position);
            priceText.gameObject.SetActive(false);
        }
    }
}
