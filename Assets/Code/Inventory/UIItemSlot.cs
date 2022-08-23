
using System;
using Code;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public UIItem storedUIItem;
    [SerializeField] protected Sprite normalTexture;
    [SerializeField] protected Sprite favoriteTexture;
    protected Image image;
    protected bool favorite;
    private ItemSlot listenToItemSlot;
    protected void Awake()
    {
        image = GetComponent<Image>();
        
    }

    private void Start()
    {
        Instantiate(storedUIItem, transform.position, Quaternion.identity, GetComponent<Canvas>().transform);
    }

    public void ListenToItemSlot(ItemSlot newItemSlot)
    {
        if (listenToItemSlot != null)
        {
            listenToItemSlot.OnItemReceived -= SetItemDisplay;
            listenToItemSlot.OnItemRemoved -= HideItem;
        }
        newItemSlot.OnItemReceived += SetItemDisplay; 
        newItemSlot.OnItemRemoved += HideItem;
        listenToItemSlot = newItemSlot;
    }

    private void SetItemDisplay(InventoryItem item)
    {
        storedUIItem.SetDisplay(image.transform.position, item.StackAmount, item.Id);
    }

    private void HideItem()
    {
        
    }
    public bool Favorite
    {
        get => favorite;
        private set => favorite = value;
    }
    public void ToggleFavorite()
    {
        if (!listenToItemSlot.IsEmpty())
        {
            favorite = !favorite;
            image.sprite = favorite ? favoriteTexture : normalTexture;
            storedUIItem.favorite = favorite;
        }
    }
}
