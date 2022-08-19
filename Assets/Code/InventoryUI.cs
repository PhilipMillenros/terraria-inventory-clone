using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private GenericInventory inventory;
    void Start()
    {
        inventory = new GenericInventory(50);
        
    }

    private void Tests()
    {
        Debug.Log("Initialization" + (InitializeTest() ? "Passed" : "Failed"));
        Debug.Log("SetItemSlots" + (SetItemSlotsTest() ? "Passed" : "Failed"));
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

    
}
