using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class ChestInventory : InventoryUI
{
    protected new void Start()
    {
        base.Start();
        SetInventoryDisplay(false);
    }
}
