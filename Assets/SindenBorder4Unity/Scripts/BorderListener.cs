using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SindenUnity
{
    [RequireComponent(typeof(RectTransform))]
    public class BorderListener : MonoBehaviour
    {
        public bool IsReady
        {
            get
            {
                RectTransform rect = transform as RectTransform;
                return rect.anchorMin.x == 0 && rect.anchorMin.y == 0 && rect.anchorMax.x == 1 && rect.anchorMax.y == 1;
            }
        }

        private void OnEnable()
        {
            SindenBorder.OnBorderUpdated += UpdateCanvas;
        }

        private void OnDisable()
        {
            SindenBorder.OnBorderUpdated -= UpdateCanvas;
        }

        public void UpdateCanvas(BorderPropObject border)
        {
            RectTransform rect = transform as RectTransform;

            float height = border.uniformSize ? border.width : border.height;
            rect.sizeDelta = new Vector2(border.width * -2, height * -2);
        }
    }
}