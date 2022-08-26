using System;
using System.Collections;
using Code;
using Code.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite favoriteCursor, normalCursor;
    [HideInInspector] public UIItemSlot mouseUIItemSlot;
    [SerializeField] private Transform itemOrigin;
    [SerializeField] private Image cursorImage;
    private Vector3 position;
    
    private Transform heldItemOrigin;
    public Vector3 Position
    {
        get
        {
            position = Input.mousePosition;
            return position;
        }
        private set => position = value;
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
        mouseUIItemSlot = gameObject.GetComponentInChildren<UIItemSlot>();
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

    private void OnItemSlotClick(ItemSlot itemSlotClicked, PointerEventData clickInfo)
    {
        if (clickInfo.button == PointerEventData.InputButton.Left)
        {
            LeftClickActions(itemSlotClicked);
        }

        if (clickInfo.button == PointerEventData.InputButton.Right)
        {
            RightClickActions(itemSlotClicked);
        }
    }

    private void LeftClickActions(ItemSlot itemSlotClicked)
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
            if (itemSlotClicked.Item.Id == mouseItemSlot.Item.Id)
            {
                GenericInventory.StackItems(itemSlotClicked.Item, mouseItemSlot.Item);
                Debug.Log("Stacked");
            }
            else
            {
                GenericInventory.SwapItems(itemSlotClicked, mouseItemSlot);
                Debug.Log("Swapped");
            }

            
            return;
        }

        if (!itemSlotClicked.IsEmpty() && mouseItemSlot.IsEmpty())
        {
            mouseItemSlot.SetItem(itemSlotClicked.Item);
            Debug.Log("Taken");
            return;
        }
    }

    private void RightClickActions(ItemSlot itemSlotClicked)
    {
        ItemSlot mouseItemSlot = mouseUIItemSlot.displayedItemSlot;
        if (!itemSlotClicked.IsEmpty())
        {
            if (mouseItemSlot.IsEmpty() || itemSlotClicked.Item.Id == mouseItemSlot.Item.Id)
            {
                StartCoroutine(RapidlyTransferItems(itemSlotClicked));
            }
        }
    }

    private IEnumerator RapidlyTransferItems(ItemSlot itemSlotClicked)
    {
        ItemSlot mouseItemSlot = mouseUIItemSlot.displayedItemSlot;
        float delay = 0.3f;
        while (!itemSlotClicked.IsEmpty() && Input.GetMouseButton(1))
        {
            itemSlotClicked.TransferItems(mouseItemSlot, 1);
            yield return new WaitForSeconds(Mathf.Clamp(delay, 0.03f, 1));
            delay -= 0.05f;
        }
    }
}
