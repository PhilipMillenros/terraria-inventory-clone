using System;

namespace Code
{
    public class InventoryItem
    {
        public int Id { get; private set; }
        private int stackAmount = 1;
        private bool favorite = false;
        
        public int StackAmount 
        { 
            get => stackAmount; 
            set 
            {
                stackAmount = value;
                if (StackAmount < 1) 
                {
                    DetachFromItemSlot();
                    return;
                }
                ItemSlot?.ItemValuesUpdated();
            } 
        }
        public int MaxStackAmount { get; private set; }
        private ItemSlot itemSlot;
        public Action OnItemUpdated;
        public ItemSlot ItemSlot
        {
            get => itemSlot;
            private set => itemSlot = value;
        }
        public InventoryItem(int id, int stackAmount, ItemSlot itemSlot, int maxStackAmount = 999)
        {
            Id = id;
            StackAmount = stackAmount;
            MaxStackAmount = maxStackAmount;
            ItemSlot = itemSlot;
        }
        public bool IsFavorite()
        {
            return favorite;
        }
        public void SetFavorite(bool value)
        {
            favorite = value;
            ItemSlot?.ItemValuesUpdated();
        }
        public void ToggleFavorite()
        {
            SetFavorite(!favorite);
        }
        
        public bool SetItemSlot(ItemSlot newItemSlot)
        {
            if (!newItemSlot.FulfillsHoldRequirements(this))
            {
                return false;
            }
            if (newItemSlot.Item != this)
            {
                newItemSlot.RemoveItem();
            }
            ItemSlot = newItemSlot;
            if (ItemSlot.Item != this && newItemSlot != null)
            {
                newItemSlot.HoldItem(this);
            }
            return true;
        }

        public void DetachFromItemSlot()
        {
            if (itemSlot != null && !itemSlot.IsEmpty())
            {
                itemSlot.RemoveItem();
            }
            itemSlot = null;
        }
    }
}