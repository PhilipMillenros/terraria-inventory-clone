
using Code;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public UIItem storedUIItem;
    [SerializeField] protected Sprite normalTexture;
    [SerializeField] protected Sprite favoriteTexture;
    protected Image image;
    protected bool favorite;
    public ItemSlot itemSlot;
    protected void Awake()
    {
        image = GetComponent<Image>();
    }
    public bool Favorite
    {
        get => favorite;
        private set => favorite = value;
    }
    public void ToggleFavorite()
    {
        if (!itemSlot.IsEmpty())
        {
            favorite = !favorite;
            image.sprite = favorite ? favoriteTexture : normalTexture;
            storedUIItem.favorite = favorite;
        }
    }
}
