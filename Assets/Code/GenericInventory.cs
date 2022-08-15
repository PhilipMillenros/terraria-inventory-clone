
namespace Code
{
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class GenericInventory
    {
        private ItemSlot[] inventoryItems;

        public GenericInventory(int inventorySlots)
        {
            inventoryItems = new ItemSlot[inventorySlots];
        }   
        public ItemSlot this[int index]
        {
            get => inventoryItems[index];
        }
        public void Sort()
        {
            QuickSort(inventoryItems, 0, inventoryItems.Length - 1);
        }
        public InventoryItem PopItem(int index)
        {
            InventoryItem poppedItem = inventoryItems[index].Item;
            inventoryItems[index] = null;
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
            ItemSlot temp = inventoryItems[firstIndex];
            inventoryItems[firstIndex] = inventoryItems[secondIndex];
            inventoryItems[secondIndex] = temp;
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
        
        public InventoryItem PopItem()
        {
            InventoryItem poppedItem = Item;
            Item = null;
            return poppedItem;
        }
    }
}