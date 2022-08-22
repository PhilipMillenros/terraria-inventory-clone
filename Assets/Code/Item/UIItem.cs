using Code;
using TMPro;
using UnityEngine;


public class UIItem : MonoBehaviour
{
    public enum Rarity { White, Green, Pink, Lime, Purple };
    public enum Type { Tile, Consumable, Furniture, Weapon, Tool, Equipable };
    
    [SerializeField] public Rarity rarity = Rarity.White;
    [SerializeField] public Type type;
    [SerializeField] private int quantity = 1;
    [SerializeField] public bool stackable;
    [HideInInspector] public bool favorite;
    [SerializeField] TextMeshProUGUI quantityText;
    private void Start()
    {
        SetupQuantityText();
        gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void SetupQuantityText()
    {
        if (!stackable)
        {
            return;
        }
        quantityText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        quantityText.text = quantity.ToString();
    }
}
