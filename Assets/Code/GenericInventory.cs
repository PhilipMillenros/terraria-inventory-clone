
using System.Runtime.CompilerServices;
using UnityEngine.Analytics;

namespace Code
{
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class GenericInventory
    {
        private ItemSlot[] itemSlots;

        public GenericInventory(int inventorySlots)
        {
            itemSlots = new ItemSlot[inventorySlots];
        }

        public void AddNewItemSlots(int amount)
        {
            int totalItemSlots = amount + itemSlots.Length;
            ItemSlot[] newItemSlots = new ItemSlot[totalItemSlots];
            InventoryItem[] items = FindAllItems();
            for (int i = 0; i < itemSlots.Length; i++)
            {
                newItemSlots[i] = itemSlots[i];
            }
            itemSlots = newItemSlots;
        }
        
        public ItemSlot this[int index]
        {
            get => itemSlots[index];
        }
        public void Sort()
        {
            StackDuplicateItems();
            InventoryItem[] items = FindAllItems();
            QuickSort(items, 0, items.Length - 1);
            
        }

        private InventoryItem[] FindAllItems()
        {
            int itemCount = GetOccupiedSlotsCount();
            
            InventoryItem[] items = new InventoryItem[itemCount];
            int pivot = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].IsEmpty())
                {
                    items[pivot] = itemSlots[i].Item;
                    pivot++;
                }
            }
            return items;
        }

        private int GetOccupiedSlotsCount()
        {
            int itemCount = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemCount += itemSlots[i].IsEmpty() ? 0 : 1;
            }

            return itemCount;
        }
        public void StackDuplicateItems()
        {
            var duplicateItems = new Dictionary<int, List<InventoryItem>>();
            List<int> uniqueIds = new List<int>();
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].IsEmpty()) continue;

                int itemId = itemSlots[i].Item.Id;
                if (duplicateItems.ContainsKey(itemId))
                {
                    duplicateItems.Add(itemId, new List<InventoryItem>());
                    uniqueIds.Add(itemId);
                }
                duplicateItems[itemId].Add(itemSlots[i].Item); 
            }

            for (int i = 0; i < uniqueIds.Count; i++)
            {
                int id = uniqueIds[i];
                
                if (duplicateItems[id].Count < 2) continue;
                
                for (int y = 0; y < duplicateItems[id].Count; y++)
                {
                    StackItems(duplicateItems[id][y + 1], duplicateItems[id][y]);
                }
            }
        }
        private void StackItems(InventoryItem receivingItem, InventoryItem givingItem)
        {
            Debug.Assert(receivingItem.Id != receivingItem.Id, "Can't merge items with different ids");
            
            int maxStackAmount = receivingItem.MaxStackAmount;
            receivingItem.StackAmount += givingItem.StackAmount;
            
            if (receivingItem.StackAmount > maxStackAmount)
            {
                givingItem.StackAmount = receivingItem.StackAmount - maxStackAmount;
                receivingItem.StackAmount = maxStackAmount;
            }

            if (givingItem.StackAmount < 1)
            {
                givingItem.ItemSlot.DiscardItem();
            }
        }
        private void QuickSort(InventoryItem[] items, int left, int right) //Technically QS shouldn't belong to Inventory
        {
            int i = left;
            int y = right;
            int pivot = left;
            while (i <= y)
            {
                while (items[i].Id < items[pivot].Id)
                {
                    i++;
                }

                while (items[y].Id > items[pivot].Id)
                {
                    y--;
                }

                if (i > y) continue;
                Swap(items, i, y);
                i++;
                y--;
            }
            if (left < y)
            {
                QuickSort(items, left, y);
            }
            if (right > i)
            {
                QuickSort(items, i, right);
            }
        }

        private void Swap<T>(T[] array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        public void SwapItems(int firstIndex, int secondIndex)
        {
            InventoryItem temp = itemSlots[firstIndex].Item;
            itemSlots[firstIndex].SetItem(itemSlots[secondIndex].Item);
            itemSlots[secondIndex].SetItem(temp);
        }
        public void SwapItems(ItemSlot itemSlot1, ItemSlot itemSlot2) 
        {
            InventoryItem temp = itemSlot1.Item;
            itemSlot1.SetItem(itemSlot2.Item);
            itemSlot2.SetItem(temp);
        }

        public void MoveItem(ItemSlot from, ItemSlot to)
        {
            if (!to.IsEmpty())
            {
                SwapItems(from, to);
            }
            to.SetItem(from.TakeItem());
        }
    }
    

    public abstract class InventoryItem
    {
        public int Id { get; private set; }
        public int StackAmount { get; set; }

        public int MaxStackAmount { get; private set; }
        private ItemSlot itemSlot;
        public ItemSlot ItemSlot
        {
            get => itemSlot;
            private set => itemSlot = value;
        }
        public void SetItemSlot(ItemSlot newItemSlot)
        {
            ItemSlot = newItemSlot;
            if (ItemSlot.Item != this)
            {
                newItemSlot.SetItem(this);
            }
        }
    }
    public class ItemSlot
    {
        private InventoryItem item;

        public InventoryItem Item
        {
            get => item;
            private set => item = value;
        }

        public bool IsEmpty()
        {
            return Item == null;
        }
        public void SetItem(InventoryItem newItem)
        {
            Item = newItem;
            if (newItem != null && newItem.ItemSlot != this)
            {
                newItem.SetItemSlot(this);
            }
        }
        /// <summary>
        /// Returns item and empties the item slot.
        /// </summary>
        public InventoryItem TakeItem()
        {
            InventoryItem takenItem = Item;
            Item = null;
            return takenItem;
        }
        
        public void DiscardItem()
        {
            Item = null;
        }
    }
}