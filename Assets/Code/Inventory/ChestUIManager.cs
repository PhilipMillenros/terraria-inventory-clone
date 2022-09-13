using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestUIManager : MonoBehaviour
{
    //Manages which chest is displayed and which to trade items with
    private ChestInventory currentChestDisplayed;

    public ChestInventory CurrentChestDisplayed
    {
        get => currentChestDisplayed;
    }

    public void OpenChest(ChestInventory chest)
    {
        if (currentChestDisplayed != null)
        {
            currentChestDisplayed.SetInventoryDisplay(false);
        }
        currentChestDisplayed = chest;
        currentChestDisplayed.SetInventoryDisplay(true);
    }

    public void CloseChest()
    {
        if (currentChestDisplayed == null)
        {
            return;
        }
        currentChestDisplayed.SetInventoryDisplay(false);
        currentChestDisplayed = null;
    }
}
