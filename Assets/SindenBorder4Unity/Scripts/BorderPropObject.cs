using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SindenUnity
{
    [CreateAssetMenu(fileName = "New Border Properties Object", menuName = "SindenBorder4Unity/Border Properties Object", order = 1)]
    public class BorderPropObject : ScriptableObject
    {
        [System.Serializable]
        public struct BorderProperties
        {
            public float widths;
            public float height;
            public bool uniformSize;
            public Color color;
        }

        public float width
        {
            get
            {
                return borderProperties.widths;
            }
            set
            {
                borderProperties.widths = value;
            }
        }
        public float height
        {
            get
            {
                return borderProperties.height;
            }
            set
            {
                borderProperties.height = value;
            }
        }
        public bool uniformSize
        {
            get
            {
                return borderProperties.uniformSize;
            }
            set
            {
                borderProperties.uniformSize = value;
            }
        }

        public Color color
        {
            get
            {
                return borderProperties.color;
            }
            set
            {
                borderProperties.color = value;
            }
        }

        [SerializeField] [HideInInspector] BorderProperties borderProperties;

        public void Reset()
        {
            uniformSize = true;
            color = Color.white;
            width = 5;
            height = 5;
        }
    }
}