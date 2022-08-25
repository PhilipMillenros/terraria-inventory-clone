
using System;
using Code;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IPointerClickHandler
{
    public GameObject UIItemPrefab;
    private UIItem storedItem;
    [SerializeField] protected Sprite normalTexture;
    [SerializeField] protected Sprite favoriteTexture;
    protected bool favorite;
    public ItemSlot displayedItemSlot;
    public static event Action<ItemSlot, PointerEventData> OnClickEvent;

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
            storedItem.gameObject.SetActive(true);
            storedItem.DisplayItemValues(transform.position, item.StackAmount, item.Id);
        }
    }

    private void HideItem()
    {
        storedItem.gameObject.SetActive(false);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickEvent.Invoke(displayedItemSlot, eventData);
    }
}
