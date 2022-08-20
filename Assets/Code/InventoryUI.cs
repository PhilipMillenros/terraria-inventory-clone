using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private GenericInventory inventory;
    void Start()
    {
        Tests();
    }

    private void Tests()
    {
        Debug.Log("Initialization: " + (InitializeTest() ? "Passed" : "Failed"));
        Debug.Log("Set Item Slots: " + (SetItemSlotsTest() ? "Passed" : "Failed"));
        Debug.Log("Sort Items: " + (SortTest() ? "Passed" : "Failed"));
    }

    private bool InitializeTest()
    {
        GenericInventory testInventory = new GenericInventory(100);
        for (int i = 0; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    private bool SetItemSlotsTest()
    {
        GenericInventory testInventory = new GenericInventory(5);
        for (int i = 0; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i] == null)
            {
                return false;
            }
        }
        testInventory.SetItemSlotsCount(12);
        for (int i = 0; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i] == null)
            {
                return false;
            }
        }
        testInventory.SetItemSlotsCount(3);
        for (int i = 0; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i] == null)
            {
                return false;
            }
        }
        return true;
    }

    private bool SortTest()
    {
        GenericInventory testInventory = new GenericInventory(5);
        testInventory.Sort();
        for (int i = 1; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i - 1].Item.Id > testInventory[i].Item.Id)
            {
                return false;
            }
        }

        for (int i = 0; i < testInventory.ItemSlotsCount; i++)
        {
           Debug.Log(testInventory[i]?.Item.StackAmount);
        }
        return true;
    } 

    
}
