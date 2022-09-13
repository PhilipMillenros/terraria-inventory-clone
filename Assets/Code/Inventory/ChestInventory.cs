using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class ChestInventory : InventoryUI<GenericInventory>
{
    private void Start()
    {
        base.Start();
        SetInventoryDisplay(false);
    }
}
