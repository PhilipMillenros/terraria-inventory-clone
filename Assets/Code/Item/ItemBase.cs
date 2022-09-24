using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public enum Rarity { White, Green, Pink, Lime, Purple };
    public enum Type { Tile, Consumable, Furniture, Weapon, Tool, Equipable };
    
    [SerializeField] public Rarity rarity = Rarity.White;
    [SerializeField] public Type type;
    [SerializeField] private int stackAmount = 1;
    [HideInInspector] public bool favorite;
    [SerializeField] TextMeshProUGUI quantityText;
    
    public int MaxStack = 999;
    public int Id;
    public int StackAmount
    { 
        get => stackAmount; 
        private set
        {
            stackAmount = value;
            quantityText.text = stackAmount == 1 ? "" : stackAmount.ToString();
        }
    }
    private void Start()
    {
        SetupQuantityText();
        gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
    }
    private void SetupQuantityText()
    {
        if (MaxStack < 2)
        {
            return;
        }
        quantityText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        quantityText.text = stackAmount.ToString();
    }
}
