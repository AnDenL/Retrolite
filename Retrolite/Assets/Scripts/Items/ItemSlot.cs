using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ItemStack itemStack;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Animator animator;

    private void Start() => animator = GetComponent<Animator>();

    public void SetItem(ItemStack newItem)
    {
        itemStack = newItem;
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count.ToString();
        UpdateUI();
    }

    public void UpdateUI()
    {
        image.sprite = itemStack.Item.Icon;
        countText.text = itemStack.Count > 1 ? itemStack.Count.ToString() : string.Empty;
    }

    public void SelectAnimation() => animator.SetTrigger("OnSelect");
    
    public void Clear()
    {
        itemStack = null;
        image.sprite = null;
        image.enabled = false;
        countText.text = "";
    }
}