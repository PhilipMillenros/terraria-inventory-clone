
using System;
using Code;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] protected Sprite normalItemSlotSprite;
    [SerializeField] protected Sprite favoriteItemSlotSprite;
    
    private bool favorite;
    private Image itemSlotImage;
    private UIItem storedItem;
    
    public ItemSlot VisualizedItemSlot;
    public GameObject UIItemPrefab; 
    public static event Action<ItemSlot, PointerEventData> OnClickEvent;

    private void Awake()
    {
        itemSlotImage = GetComponent<Image>();
    }

    public void VisualizeItemSlot(ItemSlot newItemSlot)
    {
        UnsubscribePreviousItemSlot();
        VisualizedItemSlot = newItemSlot;
        ListenToNewItemSlot(newItemSlot);
        if (!newItemSlot.IsEmpty())
        {
            UpdateHeldItemVisuals(newItemSlot.Item);
        }
        else
        {
            HideItem();
        }
    }

    private void UnsubscribePreviousItemSlot()
    {
        if (VisualizedItemSlot != null)
        {
            VisualizedItemSlot.OnItemReceived -= UpdateHeldItemVisuals;
            VisualizedItemSlot.OnItemRemoved -= HideItem;
            VisualizedItemSlot.OnItemValuesUpdated -= VisualizeItemValues;
        }
    }

    private void ListenToNewItemSlot(ItemSlot newItemSlot)
    {
        newItemSlot.OnItemReceived += UpdateHeldItemVisuals; 
        newItemSlot.OnItemRemoved += HideItem;
        newItemSlot.OnItemValuesUpdated += VisualizeItemValues;
    }
    
    private void UpdateHeldItemVisuals(InventoryItem item)
    {
        if (VisualizedItemSlot == null)
        {
            return;
        }
        if (storedItem == null)
        {
            storedItem = Instantiate(UIItemPrefab, transform.position, Quaternion.identity, transform).GetComponent<UIItem>();
        }
        VisualizeItemValues(item);
        UpdateItemSlotVisuals();
    }

    private void VisualizeItemValues(InventoryItem item)
    {
        if (!VisualizedItemSlot.IsEmpty())
        {
            storedItem.gameObject.SetActive(true);
            storedItem.DisplayItemValues(transform.position, item.StackAmount, item.Id);
            
        }
        UpdateItemSlotVisuals();
    }

    
    private void HideItem()
    {
        storedItem.gameObject.SetActive(false);
        UpdateItemSlotVisuals();
    }
    private void UpdateItemSlotVisuals()
    {
        if (itemSlotImage == null)
        {
            return;
        }
        if (!VisualizedItemSlot.IsEmpty())
        {
            itemSlotImage.sprite = VisualizedItemSlot.Item.IsFavorite() ? favoriteItemSlotSprite : normalItemSlotSprite;
        }
        else
        {
            itemSlotImage.sprite = normalItemSlotSprite;
        }
    }
    public bool Favorite
    {
        get => favorite;
        private set => favorite = value;
    }
    public void ToggleFavorite()
    {
        if (!VisualizedItemSlot.IsEmpty())
        {
            favorite = !favorite;
            storedItem.Favorite = favorite;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(VisualizedItemSlot, eventData);
    }
}
