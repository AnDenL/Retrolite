using UnityEngine;
using Creatures;

[RequireComponent(typeof(AudioSource))]
public class ShopItem : Interactable
{
    public int price;

    [SerializeField] private Sprite soldSprite;
    [SerializeField] private TextMesh priceText;

    public Transform Item;
    public AudioClip ByingSound;

    private ArcAnim arc;
    private Collider2D coll;
    private AudioSource source;

    private bool isBought;

    protected override void Start()
    {
        base.Start();

        if (Item == null) Item = transform.GetChild(1);
        coll = Item.GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();

        coll.enabled = false;
        priceText.text = price.ToString();
        PlayerController.Player.Resources.Get(ResourceType.Money).OnChanged += ChangeTextColor;
        
        arc = Item.GetComponent<ArcAnim>();
        arc.sr.GetComponent<SpriteRenderer>().sortingOrder = -2;

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
            source.pitch = Random.Range(0.8f, 1.2f);
            source.PlayOneShot(ByingSound);
            isBought = true;
            sr.sprite = soldSprite;
            arc.DropTo(creature.transform.position, () => { 
                arc.sr.GetComponent<SpriteRenderer>().sortingOrder = -3;
                coll.enabled = true;
             });
            priceText.gameObject.SetActive(false);
        }
    }
}
