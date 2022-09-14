using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortChest : MonoBehaviour
{
    [SerializeField] private ChestUIManager chestUIManager;
    public void SortActiveChest()
    {
        if (chestUIManager.CurrentChestDisplayed != null)
        {
            chestUIManager.CurrentChestDisplayed.Sort();
        }
    }
}
