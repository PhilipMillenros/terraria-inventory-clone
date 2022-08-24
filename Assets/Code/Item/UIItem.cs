using Code;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIItem : MonoBehaviour
{
    [SerializeField] private int quantity = 1;
    [HideInInspector] public bool favorite;
    [SerializeField] TextMeshProUGUI itemStackText;
    [SerializeField] private Image image;
    [SerializeField] private SpriteCollection ItemCollection;
    private void Awake()
    {
        SetupQuantityText();
    }

    private void SetupQuantityText()
    {
        itemStackText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void DisplayItemValues(Vector2 position, int stackAmount, int id)
    {
        image.sprite = ItemCollection.Sprites[id];
        transform.position = position;
        SetItemStackCount(stackAmount);
    }

    private void SetItemStackCount(int value)
    {
        itemStackText.text = value.ToString();
    }
}
