using Code;
using TMPro;
using UnityEngine;


public class UIItem : MonoBehaviour
{
    [SerializeField] private int quantity = 1;
    [HideInInspector] public bool favorite;
    [SerializeField] TextMeshProUGUI itemStackText;
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
        
    }
}
