
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

namespace Code
{
    
    using System;
    using System.Collections.Generic;
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
            StackAllDuplicateItems();
            InventoryItem[] items = FindAllItems();
            QuickSort(items, 0, items.Length - 1);
            EmptyInventory();
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
        public InventoryItem[] FindAllItems()
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
        public void StackAllDuplicateItems()
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
                for (int y = 0; y < duplicateItems[uniqueIds[i]].Count; y++)
                {
                    int pivot = y;
                    int itemCount = duplicateItems[uniqueIds[i]].Count;
                    
                    InventoryItem givingItem = duplicateItems[uniqueIds[i]][pivot];
                    while (givingItem.StackAmount > 0 && ++pivot < itemCount)
                    {
                        InventoryItem receivingItem = duplicateItems[uniqueIds[i]][pivot];
                        StackItems(receivingItem, givingItem);
                    }
                }
            }
        }
        public static void StackItems(InventoryItem receivingItem, InventoryItem givingItem)
        {
            int maxStackAmount = receivingItem.MaxStackAmount;

            if (receivingItem.StackAmount >= maxStackAmount)
            {
                return;
            }

            receivingItem.StackAmount += givingItem.StackAmount;
            if (receivingItem.StackAmount >= maxStackAmount)
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
        public static void SwapItems(ItemSlot itemSlot1, ItemSlot itemSlot2) 
        {
            if (!IsSwapValid(itemSlot1, itemSlot2))
            {
                return;
            }
            InventoryItem temp = itemSlot1.Item;
            itemSlot1.SetItem(itemSlot2.Item);
            itemSlot2.SetItem(temp);
        }

        public void TransferItems(ItemSlot from, ItemSlot to, int amount)
        {
            from.TransferItems(to, amount);
        }
        public static bool IsSwapValid(ItemSlot itemSlot1, ItemSlot itemSlot2)
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
}