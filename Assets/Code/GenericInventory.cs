
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
        public ItemSlot this[int index]
        {
            get => itemSlots[index];
        }
        public void Sort()
        {
            MergeDuplicateItems();
            QuickSort(itemSlots, 0, itemSlots.Length - 1);
        }

        private void MergeDuplicateItems()
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
                float totalItemsCount = 0f;
                for (int y = 0; y < duplicateItems[uniqueIds[i]][y].Count; y++)
                {
                    
                }
            }
        }
        public InventoryItem PopItem(int index)
        {
            InventoryItem poppedItem = itemSlots[index].Item;
            itemSlots[index] = null;
            return poppedItem;
        }
        private void QuickSort(ItemSlot[] itemSlots, int left, int right)
        {
            int i = left;
            int y = right;
            int pivot = left;
            while (i <= y)
            {
                while (itemSlots[i].Item.Id < itemSlots[pivot].Item.Id)
                {
                    i++;
                }

                while (itemSlots[y].Item.Id > itemSlots[pivot].Item.Id)
                {
                    y--;
                }

                if (i > y) continue;
                Swap(i, y);
                i++;
                y--;
            }
            if (left < y)
            {
                QuickSort(itemSlots, left, y);
            }
            if (right > i)
            {
                QuickSort(itemSlots, i, right);
            }
        }
        public void Swap(int firstIndex, int secondIndex)
        {
            ItemSlot temp = itemSlots[firstIndex];
            itemSlots[firstIndex] = itemSlots[secondIndex];
            itemSlots[secondIndex] = temp;
        }
    }
    

    public abstract class InventoryItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
    public class ItemSlot
    {
        public InventoryItem Item;

        public bool IsEmpty()
        {
            return Item == null;
        }
        
        public InventoryItem TakeItem() //Returns item and empties the item slot
        {
            InventoryItem poppedItem = Item;
            Item = null;
            return poppedItem;
        }
    }
}