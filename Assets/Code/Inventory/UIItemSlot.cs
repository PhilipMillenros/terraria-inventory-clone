
using System;
using Code;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour, IPointerDownHandler
{
    public GameObject UIItemPrefab;
    private UIItem storedItem;
    [SerializeField] protected Sprite normalItemSlotSprite;
    [SerializeField] protected Sprite favoriteItemSlotSprite;
    private bool favorite;
    public ItemSlot visualizedItemSlot;
    public static event Action<ItemSlot, PointerEventData> OnClickEvent;
    private Image itemSlotImage;


    private void Awake()
    {
        itemSlotImage = GetComponent<Image>();
    }

    public void VisualizeItemSlot(ItemSlot newItemSlot)
    {
        UnsubscribePreviousItemSlot();
        visualizedItemSlot = newItemSlot;
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
        if (visualizedItemSlot != null)
        {
            visualizedItemSlot.OnItemReceived -= UpdateHeldItemVisuals;
            visualizedItemSlot.OnItemRemoved -= HideItem;
            visualizedItemSlot.OnItemValuesUpdated -= VisualizeItemValues;
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
        if (visualizedItemSlot == null)
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
        if (!visualizedItemSlot.IsEmpty())
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
        if (!visualizedItemSlot.IsEmpty())
        {
            itemSlotImage.sprite = visualizedItemSlot.Item.IsFavorite() ? favoriteItemSlotSprite : normalItemSlotSprite;
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
        if (!visualizedItemSlot.IsEmpty())
        {
            favorite = !favorite;
            storedItem.favorite = favorite;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(visualizedItemSlot, eventData);
    }
}
