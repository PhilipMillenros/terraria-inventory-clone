using System;

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
            item = new InventoryItem(1, 5, this);
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

        public void TransferItems(ItemSlot toItemSlot, int amount)
        {
            if (IsEmpty() && !toItemSlot.IsItemValid(item))
            {
                return;
            }

            InventoryItem givingItem = item;
            InventoryItem receivingItem = toItemSlot.Item;

            givingItem.StackAmount -= amount;
            receivingItem.StackAmount += amount;

            if (StackIsGreaterThanMax(receivingItem))
            {
                givingItem.StackAmount += receivingItem.StackAmount - receivingItem.StackAmount;
                receivingItem.StackAmount -= receivingItem.MaxStackAmount;
            }
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
            OnItemRemoved.Invoke();
            return discardedItem;
        }
    }
}