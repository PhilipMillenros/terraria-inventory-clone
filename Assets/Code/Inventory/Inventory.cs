
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

namespace Code
{
    
    using System;
    using System.Collections.Generic;
    public class Inventory
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
        public Inventory(int inventorySlots)
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
            InventoryItem[] items = GetAllNonFavoriteItems();
            int low = 0;
            int high = items.Length - 1;
            CustomThreeWayQuickSort(items, ref low, ref high);
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
        public InventoryItem[] GetAllNonFavoriteItems()
        {
            int itemCount = NonFavoriteItemsCount();
            
            InventoryItem[] items = new InventoryItem[itemCount];
            int pivot = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if(!itemSlots[i].IsEmpty() && !itemSlots[i].Item.IsFavorite())
                {
                    items[pivot] = itemSlots[i].Item;
                    pivot++;
                }
            }
            return items;
        }
        private void CustomThreeWayQuickSort(InventoryItem[] items, ref int low, ref int high)
        {
            if (high <= low)
            {
                return;
            }
            int lowest = low;
            int highest = high;
            
            Partition(items, ref low, ref high);
            SortMiddlePartition(items, low, high);
            high++;
            low--;
            
            CustomThreeWayQuickSort(items, ref lowest, ref low);
            CustomThreeWayQuickSort(items, ref high, ref highest);
        }
        private void Partition(InventoryItem[] items, ref int low, ref int high)
        {
            int i = low;
            int pivot = items[high].Id;
            while (i <= high)
            {
                if (items[i].Id < pivot)
                {
                    Swap(items,i, low);
                    i++;
                    low++;
                }
                else if (items[i].Id > pivot)
                {
                    Swap(items,i, high);
                    high--;
                }
                else
                {
                    i += 1;
                }
            }
        }
        private void SortMiddlePartition(InventoryItem[] items, int low, int high)
        {
            int lowestStack = GetLowestStackIndex(items, low, high);
            Swap(items, lowestStack, high);
        }

        private int GetLowestStackIndex(InventoryItem[] array, int low, int high)
        {
            int lowestStackIndex = low;
            for (int i = low; i < high + 1; i++)
            {
                if (array[low].Id != array[i].Id)
                {
                    continue;
                }
                if (array[i].StackAmount < array[lowestStackIndex].StackAmount)
                {
                    lowestStackIndex = i;
                }
            }
            return lowestStackIndex;
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
            return itemSlot1.ValidHoldRequirements(itemSlot2.Item) && itemSlot2.ValidHoldRequirements(itemSlot1.Item);
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
        public void StackNonFavoriteDuplicateItems()
        {
            InventoryItem[] items = GetAllNonFavoriteItems();
            
            int[] totalStackPerId = new int[items.Length];
            
            for (int i = 0; i < totalStackPerId.Length; i++)
            {
                totalStackPerId[items[i].Id] += items[i].StackAmount;
            }
            
            int maxStackAmount = items[0].MaxStackAmount;
            
            for (int i = 0; i < items.Length; i++)
            {
                int stackValue = Mathf.Clamp( totalStackPerId[items[i].Id],0, maxStackAmount);
                items[i].StackAmount = stackValue;
                totalStackPerId[items[i].Id] -= stackValue;
                if (items[i].StackAmount < 1)
                {
                    items[i].DetachFromItemSlot();
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
    }
}