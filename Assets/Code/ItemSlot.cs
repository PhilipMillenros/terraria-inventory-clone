using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code
{
    public class ItemSlot
    {
        private InventoryItem item;
        public event Action OnItemRemoved;
        public event Action<InventoryItem> OnItemValuesUpdated;
        public event Action<InventoryItem> OnItemReceived;
        
        public InventoryItem Item
        {
            get => item;
            private set => item = value;
        }
        
        public ItemSlot()
        {
            item = new InventoryItem(Random.Range(0, 3), Random.Range(4, 400), this);
        }
        
        public bool IsEmpty()
        {
            return Item == null;
        }

        public void ItemValuesUpdated()
        {
            OnItemValuesUpdated?.Invoke(Item);
        }
        /// <summary>
        /// Sets the inventory slot item, returns if the operation succeeded or not
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public bool HoldItem(InventoryItem newItem)
        {
            if (!ValidHoldRequirements(newItem))
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
                receivingItemSlot.HoldItem(new InventoryItem(givingItemSlot.Item.Id, amount, null));
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
            
            if (!receivingItemSlot.IsEmpty() && !Inventory.ItemsIdMatches(givingItemSlot.Item, receivingItemSlot.Item))
            {
                return false;
            }

            if (!givingItemSlot.ValidHoldRequirements(receivingItemSlot.Item) || !receivingItemSlot.ValidHoldRequirements(givingItemSlot.item))
            {
                return false;
            }
            int totalAmountAfterTransfer = amount + (receivingItemSlot.IsEmpty() ? 0 : receivingItemSlot.Item.StackAmount);
            
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

        //Derived item slots might need item requirements, example: armor slots only allow armor items
        public virtual bool ValidHoldRequirements(InventoryItem item)
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