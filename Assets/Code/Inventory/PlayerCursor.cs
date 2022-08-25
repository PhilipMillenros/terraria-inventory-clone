using System;
using System.Collections;
using Code;
using Code.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite favoriteCursor, normalCursor;
    [HideInInspector] public UIItemSlot mouseUIItemSlot;
    
    [SerializeField] private Image cursorImage;
    private static Vector3 _position;
    
    private bool _holdingItem = false;
    private Transform heldItemOrigin;
    public static Vector3 Position
    {
        get
        {
            _position = Input.mousePosition;
            return _position;
        }
        private set => _position = value;
    }
    private void Awake()
    {
        Cursor.visible = false;
        heldItemOrigin = transform.GetChild(0);
        transform.SetAsLastSibling();
        SetupMouseItemSlot();
    }

    private void SetupMouseItemSlot()
    {
        mouseUIItemSlot = gameObject.GetComponent<UIItemSlot>();
        mouseUIItemSlot.DisplayItemSlot(new ItemSlot());
        UIItemSlot.OnClickEvent += OnItemSlotClick;
    }
    private void Update()
    {
        SetCursorPosition();
        SetCursorSprite();
        transform.SetAsLastSibling();
    }

    private void SetCursorPosition()
    {
        transform.position = Position;
    }

    private void SetCursorSprite()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            cursorImage.sprite = favoriteCursor;
        else
            cursorImage.sprite = normalCursor;
    }

    private void OnItemSlotClick(ItemSlot itemSlotClicked)
    {
        ItemSlot mouseItemSlot = mouseUIItemSlot.displayedItemSlot;
        if (itemSlotClicked.IsEmpty() && !mouseItemSlot.IsEmpty())
        {
            itemSlotClicked.SetItem(mouseItemSlot.Item);
            Debug.Log("Given");
            return;
        }

        if (!itemSlotClicked.IsEmpty() && !mouseItemSlot.IsEmpty())
        {
            GenericInventory.SwapItems(itemSlotClicked, mouseItemSlot);
            Debug.Log("Swapped");
            return;
        }

        if (!itemSlotClicked.IsEmpty() && mouseItemSlot.IsEmpty())
        {
            mouseItemSlot.SetItem(itemSlotClicked.Item);
            Debug.Log("Taken");
            return;
        }
    }
}
