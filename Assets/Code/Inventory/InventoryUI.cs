using System.Collections.Generic;
using Code;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    protected UIItemSlot[] UIItemSlots;
    
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 origin;
    [SerializeField] private Canvas canvas;

    protected void Start()
    {
        SetupInventory();
    }
    private void SetupInventory()
    {
        inventory = new Inventory(rows * columns);
        UIItemSlots = new UIItemSlot[rows * columns];
        for (int i = 0; i < rows * columns; i++)
        {
            UIItemSlots[i] = Instantiate(inventorySlotPrefab, CalculateItemSlotPosition(i),
                Quaternion.identity, canvas.transform).GetComponent<UIItemSlot>();
            UIItemSlots[i].VisualizeItemSlot(inventory[i]);
        }
    }
    private Vector2 CalculateItemSlotPosition(int itemSlotIndex)
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
    public void StackItems()
    {
        inventory.StackNonFavoriteDuplicateItems();
        InventoryItem[] items = inventory.GetAllNonFavoriteItems();
        inventory.EmptyInventory();
        inventory.MoveItemsToFirstAvailableSlot(items);
    }
    public void Sort()
    {
        StackItems();
        inventory.Sort();
    }
    public void SetInventoryDisplay(bool display)
    {
        int length = rows * columns;
        for (int i = 0; i < length; i++)
        {
            UIItemSlots[i].gameObject.SetActive(display);
        }
    }
}
