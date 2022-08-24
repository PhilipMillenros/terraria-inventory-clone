
using System;
using Code;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public GameObject UIItemPrefab;
    private UIItem storedItem;
    [SerializeField] protected Sprite normalTexture;
    [SerializeField] protected Sprite favoriteTexture;
    protected bool favorite;
    private ItemSlot displayedItemSlot;
    protected void Awake()
    {
    }

    public void DisplayItemSlot(ItemSlot newItemSlot)
    {
        if (displayedItemSlot != null)
        {
            displayedItemSlot.OnItemReceived -= SetItemDisplay;
            displayedItemSlot.OnItemRemoved -= HideItem;
        }
        newItemSlot.OnItemReceived += SetItemDisplay; 
        newItemSlot.OnItemRemoved += HideItem;
        displayedItemSlot = newItemSlot;
        if (!newItemSlot.IsEmpty())
        {
            SetItemDisplay(newItemSlot.Item);
        }
        else
        {
            HideItem();
        }
    }

    private void SetItemDisplay(InventoryItem item)
    {
        if (storedItem == null)
        {
            storedItem = Instantiate(UIItemPrefab, transform.position, Quaternion.identity, transform).GetComponent<UIItem>();
        }

        if (!displayedItemSlot.IsEmpty())
        {
            storedItem.enabled = true;
            storedItem.DisplayItemValues(transform.position, item.StackAmount, item.Id);
        }
    }

    private void HideItem()
    {
        storedItem.enabled = false;
    }
    public bool Favorite
    {
        get => favorite;
        private set => favorite = value;
    }
    public void ToggleFavorite()
    {
        if (!displayedItemSlot.IsEmpty())
        {
            favorite = !favorite;
            storedItem.favorite = favorite;
        }
    }
}
