using UnityEngine;

public class ChestButton : MonoBehaviour
{
    [SerializeField] private ChestUIManager chestUIManager;
    [SerializeField] private ChestInventory chest;
    public void ToggleChest()
    {
        Debug.Assert(chestUIManager != null || chest != null, "Chest button null fields");
        if (chestUIManager.CurrentChestDisplayed == chest)
        {
            chestUIManager.CloseChest();
        }
        else
        {
            chestUIManager.OpenChest(chest);
        }
    }
    
    
}

