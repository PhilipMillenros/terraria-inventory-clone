
using UnityEngine;

namespace Code.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public UIItem[] items;
        public static ItemManager Instance;
        private void Awake()
        {
            Instance = this;
        }
    }
}
