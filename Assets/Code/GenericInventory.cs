
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
        public int ItemSlotsCount
        {
            get => itemSlots.Length;
            set => SetItemSlotsCount(value);
        }
        public ItemSlot this[int index]
        {
            get => itemSlots[index];
        }
        public GenericInventory(int inventorySlots)
        {
            itemSlots = new ItemSlot[inventorySlots];
            InitializeArray(itemSlots, () => new ItemSlot());
        }

        private void InitializeArray<T>(T[] array, Func<T> provider, int startIndex = 0)
        {
            for (int i = startIndex; i < array.Length; i++)
            {
                array[i] = provider();
            }
        }
        
        
        public void Sort()
        {
            StackDuplicateItems();
            InventoryItem[] items = FindAllItems();
            QuickSort(items, 0, items.Length - 1);
            MoveItemsCloseToFirstIndex(items);
        }

        public void MoveItemsCloseToFirstIndex(InventoryItem[] items)
        {
            int itemIndex = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemIndex > items.Length - 1)
                {
                    break;
                }
                if (itemSlots[i].IsEmpty())
                {
                    itemSlots[i].SetItem(items[itemIndex]);
                    itemIndex++;
                }
            }
        }
        private void AddItemsToInventory(InventoryItem[] items)
        {
            int pivot = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (pivot >= items.Length)
                {
                    break;
                }
                if (itemSlots[i].IsEmpty())
                {
                    itemSlots[i].SetItem(items[pivot]);
                    pivot++;
                }
            }
        }
        public void EmptyInventory()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].RemoveItem();
            }
        }
        public InventoryItem[] GetAllItemsAndEmptyInventory()
        {
            InventoryItem[] items = FindAllItems();
            EmptyInventory();
            return items;
        }
        private InventoryItem[] FindAllItems()
        {
            int itemCount = OccupiedSlotsCount();
            
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

        private int OccupiedSlotsCount()
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
                if (!duplicateItems.ContainsKey(itemId))
                {
                    duplicateItems.Add(itemId, new List<InventoryItem>());
                    uniqueIds.Add(itemId);
                }
                duplicateItems[itemId].Add(itemSlots[i].Item);
            }
            
            for (int i = 0; i < uniqueIds.Count; i++)
            {
                for (int j = 0; j < duplicateItems[uniqueIds[i]].Count; j++)
                {
                    int index = j;
                    int itemCount = duplicateItems[uniqueIds[i]].Count;
                    
                    InventoryItem givingItem = duplicateItems[uniqueIds[i]][index];
                    while (givingItem.StackAmount > 0 && ++index < itemCount)
                    {
                        InventoryItem receivingItem = duplicateItems[uniqueIds[i]][index];
                        StackItems(receivingItem, givingItem);
                    }
                }
            }
        }
        private void StackItems(InventoryItem receivingItem, InventoryItem givingItem)
        {
            int maxStackAmount = receivingItem.MaxStackAmount;

            if (receivingItem.StackAmount >= maxStackAmount)
            {
                return;
            }

            receivingItem.StackAmount += givingItem.StackAmount;
            if (receivingItem.StackAmount > maxStackAmount)
            {
                givingItem.StackAmount = receivingItem.StackAmount - maxStackAmount;
                receivingItem.StackAmount = maxStackAmount;
            }
            else
            {
                givingItem.StackAmount = 0;
                givingItem.DetachFromItemSlot();
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
        public void SwapItems(ItemSlot itemSlot1, ItemSlot itemSlot2) 
        {
            if (!IsSwapValid(itemSlot1, itemSlot2))
            {
                return;
            }
            InventoryItem temp = itemSlot1.Item;
            itemSlot1.SetItem(itemSlot2.Item);
            itemSlot2.SetItem(temp);
        }

        public bool IsSwapValid(ItemSlot itemSlot1, ItemSlot itemSlot2)
        {
            return itemSlot1.IsItemValid(itemSlot2.Item) && itemSlot2.IsItemValid(itemSlot1.Item);
        }
        public void MoveItem(ItemSlot from, ItemSlot to)
        {
            if (!to.IsEmpty())
            {
                SwapItems(from, to);
            }
            to.SetItem(from.RemoveItem());
        }
        public void SetItemSlotsCount(int amount)
        { 
            ItemSlot[] newItemSlots = new ItemSlot[amount];
            for (int i = 0; i < amount; i++)
            {
                if (i >= itemSlots.Length)
                {
                    break;
                }
                newItemSlots[i] = itemSlots[i];
            }

            int uninitializedPartOfArray = itemSlots.Length - 1;
            if (uninitializedPartOfArray < amount - 1)
            {
                InitializeArray(newItemSlots, () => new ItemSlot(), uninitializedPartOfArray);
            }
            itemSlots = newItemSlots;
        }
    }
    

    public class InventoryItem
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

        public InventoryItem(int id, int stackAmount, ItemSlot itemSlot, int maxStackAmount = 1000)
        {
            Id = id;
            StackAmount = stackAmount;
            MaxStackAmount = maxStackAmount;
            ItemSlot = itemSlot;
        }
        public bool SetItemSlot(ItemSlot newItemSlot)
        {
            if (!newItemSlot.IsItemValid(this))
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
                newItemSlot.SetItem(this);
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
    public class ItemSlot
    {
        private InventoryItem item;
        public Action<InventoryItem> OnItemReceived;
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
            return discardedItem;
        }
    }
}