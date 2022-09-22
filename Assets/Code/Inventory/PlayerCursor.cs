using System.Collections;
using Code;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite favoriteCursorSprite, normalCursorSprite;
    [HideInInspector] public UIItemSlot mouseUIItemSlot;
    [SerializeField] private Image cursorImage;
    private Vector3 position;
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
        transform.SetAsLastSibling();
        SetupMouseItemSlot();
    }

    private void SetupMouseItemSlot()
    {
        mouseUIItemSlot = gameObject.GetComponentInChildren<UIItemSlot>();
        mouseUIItemSlot.VisualizeItemSlot(new ItemSlot());
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
            cursorImage.sprite = favoriteCursorSprite;
        else
            cursorImage.sprite = normalCursorSprite;
    }

    private void OnItemSlotClick(ItemSlot itemSlotClicked, PointerEventData clickInfo)
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            itemSlotClicked.Item.ToggleFavorite();
            return;
        }
        if (clickInfo.button == PointerEventData.InputButton.Left)
        {
            ItemSlotLeftClickActions(itemSlotClicked);
            return;
        }

        if (clickInfo.button == PointerEventData.InputButton.Right)
        {
            ItemSlotRightClickActions(itemSlotClicked);
        }
    }

    private void ItemSlotLeftClickActions(ItemSlot itemSlotClicked)
    {
        ItemSlot mouseItemSlot = mouseUIItemSlot.visualizedItemSlot;
        if (itemSlotClicked.IsEmpty() && !mouseItemSlot.IsEmpty())
        {
            itemSlotClicked.HoldItem(mouseItemSlot.Item);
            return;
        }

        if (!itemSlotClicked.IsEmpty() && !mouseItemSlot.IsEmpty())
        {
            bool idIsMatching = itemSlotClicked.Item.Id == mouseItemSlot.Item.Id;
            if (idIsMatching)
            {
                Inventory.StackItems(itemSlotClicked.Item, mouseItemSlot.Item);
            }
            else
            {
                Inventory.SwapItems(itemSlotClicked, mouseItemSlot);
            }
            return;
        }

        if (!itemSlotClicked.IsEmpty() && mouseItemSlot.IsEmpty())
        {
            mouseItemSlot.HoldItem(itemSlotClicked.Item);
        }
    }

    private void ItemSlotRightClickActions(ItemSlot itemSlotClicked)
    {
        ItemSlot mouseItemSlot = mouseUIItemSlot.visualizedItemSlot;
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
        ItemSlot mouseItemSlot = mouseUIItemSlot.visualizedItemSlot;
        float delay = 0.3f;
        while (!itemSlotClicked.IsEmpty() && Input.GetMouseButton(1))
        {
            itemSlotClicked.TransferItems(mouseItemSlot, 1);
            yield return new WaitForSeconds(Mathf.Clamp(delay, 0.03f, 1));
            delay -= 0.05f;
        }
    }
}
