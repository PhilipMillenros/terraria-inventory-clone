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
    private void Start()
    {
        SetupQuantityText();
        gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void SetupQuantityText()
    {
        itemStackText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        itemStackText.text = quantity.ToString();
    }

    public void SetDisplay(Vector2 position, int stackAmount, int id)
    {
        image.sprite = ItemCollection.Sprites[0];
    }
}
