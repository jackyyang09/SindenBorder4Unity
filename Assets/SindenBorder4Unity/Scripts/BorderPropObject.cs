using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SindenUnity
{
    [CreateAssetMenu(fileName = "New Border Properties Object", menuName = "SindenBorder4Unity/Border Properties Object", order = 1)]
    public class BorderPropObject : ScriptableObject
    {
        public float width;
        public float height;
        public bool uniformSize;
        public Color color;

        public void Reset()
        {
            uniformSize = true;
            color = Color.white;
            width = 5;
            height = 5;
        }
    }
}