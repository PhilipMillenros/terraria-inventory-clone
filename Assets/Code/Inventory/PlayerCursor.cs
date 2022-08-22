using System;
using System.Collections;
using Code.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerCursor : MonoBehaviour
{
    [SerializeField] private Sprite favoriteCursor, normalCursor;
    [HideInInspector] public UIItem heldUIItem;
    
    private Image cursorImage;
    private static Vector3 _position;
    
    public static PlayerCursor Instance;
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
    private void Start()
    {
        cursorImage = GetComponent<Image>();
        Cursor.visible = false;
        heldItemOrigin = transform.GetChild(0);
        Instance = this;
        transform.SetAsLastSibling();
    }
    private void Update()
    {
        transform.position = Position;
        if(_holdingItem)
            heldUIItem.transform.position = heldItemOrigin.transform.position;
        if (Input.GetKey(KeyCode.LeftAlt))
            cursorImage.sprite = favoriteCursor;
        else
            cursorImage.sprite = normalCursor;
    }
}
