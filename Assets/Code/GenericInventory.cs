
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
            StackNonFavoriteDuplicateItems();
            InventoryItem[] items = FindNonFavoriteItems();
            QuickSort(items, 0, items.Length - 1);
            DetachItems(items);
            MoveItemsCloseToFirstIndex(items);
        }

        private void DetachItems(InventoryItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].DetachFromItemSlot();
            }
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
                    itemSlots[i].HoldItem(items[itemIndex]);
                    itemIndex++;
                }
            }
        }

        public static bool ItemsIdMatches(InventoryItem item1, InventoryItem item2)
        {
            return item1.Id == item2.Id;
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
                    itemSlots[i].HoldItem(items[pivot]);
                    pivot++;
                }
            }
        }
        public void EmptyInventory()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].IsEmpty() && !itemSlots[i].Item.IsFavorite())
                {
                    itemSlots[i].RemoveItem();
                }
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
            int itemCount = ItemsCount();
            
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

        public int ItemsCount()
        {
            int itemCount = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemCount += itemSlots[i].IsEmpty() ? 0 : 1;
            }
            return itemCount;
        }
        public int NonFavoriteItemsCount()
        {
            int itemCount = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].IsEmpty())
                {
                    itemCount += itemSlots[i].Item.IsFavorite() ? 0 : 1;
                }
            }
            return itemCount;
        }
        public InventoryItem[] FindNonFavoriteItems()
        {
            int itemCount = NonFavoriteItemsCount();
            
            InventoryItem[] items = new InventoryItem[itemCount];
            int pivot = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].IsEmpty() && !itemSlots[i].Item.IsFavorite())
                {
                    items[pivot] = itemSlots[i].Item;
                    pivot++;
                }
            }
            return items;
        }
        public void StackNonFavoriteDuplicateItems()
        {
            var itemLists = new Dictionary<int, List<InventoryItem>>();
            List<int> uniqueIds = new List<int>();
            
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].IsEmpty() || itemSlots[i].Item.IsFavorite())
                {
                    continue;
                }
                
                int itemId = itemSlots[i].Item.Id;
                if (!itemLists.ContainsKey(itemId))
                {
                    itemLists.Add(itemId, new List<InventoryItem>());
                    uniqueIds.Add(itemId);
                }
                itemLists[itemId].Add(itemSlots[i].Item);
            }
            
            for (int i = 0; i < uniqueIds.Count; i++)
            {
                int stackSum = GetStackSum(itemLists[uniqueIds[i]]);
                int maxStack = itemLists[uniqueIds[i]][0].MaxStackAmount;
                int fullStacksCount = stackSum / maxStack;
                int stackRemainder = stackSum % maxStack;
                List<InventoryItem> duplicates = itemLists[uniqueIds[i]];
                
                int length = duplicates.Count;
                for (int y = 0; y < length; y++)
                {
                    if (fullStacksCount > 0)
                    {
                        duplicates[y].StackAmount = maxStack;
                        fullStacksCount--;
                    }
                    else if(stackRemainder > 0)
                    {
                        duplicates[y].StackAmount = stackRemainder;
                        stackRemainder -= stackRemainder;
                    }
                    else
                    {
                        duplicates[y].DetachFromItemSlot();
                    }
                }
            }
        }
        private int GetStackSum(List<InventoryItem> items)
        {
            int sum = 0;
            for (int i = 0; i < items.Count; i++)
            {
                sum += items[i].StackAmount;
            }
            return sum;
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
            itemSlot1.HoldItem(itemSlot2.Item);
            itemSlot2.HoldItem(temp);
        }

        public void TransferItems(ItemSlot from, ItemSlot to, int amount)
        {
            from.TransferItems(to, amount);
        }
        public static bool IsSwapValid(ItemSlot itemSlot1, ItemSlot itemSlot2)
        {
            return itemSlot1.FulfillsHoldRequirements(itemSlot2.Item) && itemSlot2.FulfillsHoldRequirements(itemSlot1.Item);
        }
        public void MoveItem(ItemSlot from, ItemSlot to)
        {
            if (!to.IsEmpty())
            {
                SwapItems(from, to);
            }
            to.HoldItem(from.RemoveItem());
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