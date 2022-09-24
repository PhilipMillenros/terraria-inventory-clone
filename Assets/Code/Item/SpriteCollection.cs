using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    [CreateAssetMenu(fileName = "Sprites", menuName = "ScriptableObjects/Sprites", order = 1)]
    public class SpriteCollection : ScriptableObject
    {
        [SerializeField] public List<Sprite> Sprites;
    }
}