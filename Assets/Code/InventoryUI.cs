using System.Collections.Generic;
using Code;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class InventoryUI<T> : MonoBehaviour where T : GenericInventory
{
    public T inventory;
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 origin;
    [SerializeField] private float width;
    [SerializeField] private float height;
    private int inventorySlots;
    private UIItemSlot[] UIItemSlots;
    [SerializeField] private Canvas canvas;

    
    protected void Start()
    {
        Tests();
        SetupInventory();
    }

    public void SetInventoryDisplay(bool display)
    {
        int length = rows * columns;
        for (int i = 0; i < length; i++)
        {
            UIItemSlots[i].gameObject.SetActive(display);
        }
    }
    private void SetupInventory()
    {
        inventory = (T) new GenericInventory(rows * columns);
        UIItemSlots = new UIItemSlot[rows * columns];
        for (int i = 0; i < rows * columns; i++)
        {
            UIItemSlots[i] = Instantiate(inventorySlotPrefab, GetItemSlotPosition(i),
                Quaternion.identity, canvas.transform).GetComponent<UIItemSlot>();
            UIItemSlots[i].VisualizeItemSlot(inventory[i]);
        }
    }
    private Vector2 GetItemSlotPosition(int itemSlotIndex)
    {
        var pixelRect = canvas.pixelRect;
        float canvasWidth = pixelRect.width;
        float canvasHeight = pixelRect.height;
        const int screenSizeConstant = 1000;
        float screenSizeMultiplier = canvasWidth / screenSizeConstant;
        
        return new Vector2(itemSlotIndex % rows * offset.x * screenSizeMultiplier + origin.x * screenSizeMultiplier,
            (itemSlotIndex / rows + 1) * -offset.y * screenSizeMultiplier - origin.y * screenSizeMultiplier +
            canvasHeight);
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

    public void QuickStack(T uiInventory)
    {
        
    }
    public void StackItems()
    {
        inventory.StackNonFavoriteDuplicateItems();
        InventoryItem[] items = inventory.FindNonFavoriteItems();
        inventory.EmptyInventory();
        inventory.MoveItemsCloseToFirstIndex(items);
    }
    public void Sort()
    {
        inventory.Sort();
        StackItems();
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
        GenericInventory testInventory = new GenericInventory(1000);
        testInventory.Sort();
        for (int i = 1; i < testInventory.ItemSlotsCount; i++)
        {
            if (testInventory[i - 1].Item?.Id > testInventory[i].Item?.Id)
            {
                return false;
            }
        }
        return true;
    }
}
