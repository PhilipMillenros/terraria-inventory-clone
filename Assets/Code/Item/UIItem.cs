using Code;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemStackText;
    [SerializeField] private SpriteCollection ItemCollection;
    [SerializeField] private Image image;

    [HideInInspector] public bool Favorite;
    
    private RectTransform rectTransform;
    
    private void Awake()
    {
        SetupQuantityText();
    }

    private void SetupQuantityText()
    {
        itemStackText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void DisplayItemValues(Vector2 position, int stackAmount, int id)
    {
        image.sprite = ItemCollection.Sprites[id];
        rectTransform.sizeDelta = ItemCollection.Sprites[id].rect.size * 1.4735f;
        transform.position = position;
        SetItemStackCount(stackAmount);
    }

    private void SetItemStackCount(int value)
    {
        itemStackText.text = value.ToString();
    }
}
