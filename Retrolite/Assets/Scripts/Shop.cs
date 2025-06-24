using UnityEngine;
using System.Collections;

public class Shop : Interactable
{
    public int price;

    [SerializeField]
    private Sprite soldSprite;
    [SerializeField]
    private TextMesh priceText;

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
            StartCoroutine(BuyItem(player, transform.GetChild(1)));
        }
    }

    private IEnumerator BuyItem(Player player, Transform item)
    {
        Vector2 targetPosition = (Vector2)player.transform.position + Vector2.down;
        Vector2 pos = item.position;
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1;
        item.transform.parent = null;
        priceText.gameObject.SetActive(false);

        Transform shadow = item.GetChild(0);

        float t = 0;
        float height = 0;
        while (t < 1)
        {
            height = Mathf.Sin(t * Mathf.PI) * 0.75f;
            shadow.localPosition = new Vector3(0, -height - 0.25f, 0);
            item.position = Vector3.Lerp(pos, targetPosition, t) + Vector3.up * height;
            t += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.sortingOrder = -3;
        item.GetComponent<Collider2D>().enabled = true;
    }

}
