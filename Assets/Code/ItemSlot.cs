using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
    public class ItemSlot
    {
        private InventoryItem item;
        public Action<InventoryItem> OnItemReceived;
        public Action OnItemRemoved;
        public InventoryItem Item
        {
            get => item;
            private set => item = value;
        }
        
        public ItemSlot()
        {
            item = new InventoryItem(Random.Range(0, 3), Random.Range(1, 400), this);
        }

        public bool IsEmpty()
        {
            return Item == null;
        }
        
        /// <summary>
        /// Sets the inventory slot item, returns if the operation succeeded or not
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public bool SetItem(InventoryItem newItem)
        {
            if (!IsItemValid(newItem))
            {
                return false;
            }
            item?.DetachFromItemSlot();
            Item = newItem;
            if (newItem.ItemSlot != this)
            {
                newItem.DetachFromItemSlot();
            }

            if (newItem != null && newItem.ItemSlot != this)
            {
                newItem.SetItemSlot(this);
                OnItemReceived?.Invoke(newItem);
            }

            return true;
        }

        public void TransferItems(ItemSlot receivingItemSlot, int amount)
        {
            ItemSlot givingItemSlot = this;
            if (!TransferIsValid(receivingItemSlot, givingItemSlot, amount))
            {
                return;
            }
            if (receivingItemSlot.IsEmpty())
            {
                receivingItemSlot.SetItem(new InventoryItem(givingItemSlot.Item.Id, amount, receivingItemSlot));
                givingItemSlot.Item.StackAmount -= amount;
            }
            else
            {
                receivingItemSlot.item.StackAmount += amount;
                givingItemSlot.Item.StackAmount -= amount;
            }
            
        }
        private bool TransferIsValid(ItemSlot receivingItemSlot, ItemSlot givingItemSlot, int amount)
        {
            if (givingItemSlot.IsEmpty())
            {
                return false;
            }
            if (!receivingItemSlot.IsEmpty() && givingItemSlot.Item.Id != receivingItemSlot.Item.Id)
            {
                return false;
            }
            
            int giveAmount = givingItemSlot.Item.StackAmount + amount;
            int totalAmountAfterTransfer = giveAmount + (receivingItemSlot.IsEmpty() ? 0 : receivingItemSlot.Item.StackAmount);
            
            int maxStackAmount = item.MaxStackAmount;
            if (totalAmountAfterTransfer > maxStackAmount)
            {
                return false;
            }
            return true;
        }

        private static bool StackIsGreaterThanMax(InventoryItem inventoryItem)
        {
            return inventoryItem.StackAmount > inventoryItem.MaxStackAmount;
        }

        //For derived members that has item requirements, example armor slots only allow armor items
        public virtual bool IsItemValid(InventoryItem item)
        {
            return true;
        }

        /// <summary>
        /// Returns item and empties the item slot.
        /// </summary>

        public InventoryItem RemoveItem()
        {
            InventoryItem discardedItem = Item;
            Item = null;
            discardedItem?.DetachFromItemSlot();
            OnItemRemoved?.Invoke();
            return discardedItem;
        }
    }
}