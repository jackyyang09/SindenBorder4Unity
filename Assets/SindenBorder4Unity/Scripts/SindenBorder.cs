using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SindenUnity
{
    [RequireComponent(typeof(RectTransform))]
    public class SindenBorder : MonoBehaviour
    {
        [Tooltip("Will print normal statements to the console, warnings and errors will be printed regardless of this option.")]
        [SerializeField] bool logActions = true;

        [Tooltip("Currently selected border in the border list")]
        [SerializeField] int activeIndex = 0;

        [Tooltip("The width/height of the border will be scaled by this value before being applied to the RectTransform")]
        [SerializeField] float borderScale = 1;

        [SerializeField] List<BorderPropObject> borders = null;
        public List<BorderPropObject> Borders
        {
            get
            {
                return borders;
            }
        }

        /// <summary>
        /// Returns true if there is at least 1 border stored in this component. 
        /// Also removes any null entries in the borders array
        /// </summary>
        public bool HasBorders
        { 
            get
            {
                bool borderExists = false;
                for (int i = borders.Count - 1; i > -1; i--)
                {
                    if (borders[i] == null)
                    {
                        borders.RemoveAt(i);
                    }
                    else borderExists = true;
                }
                if (activeIndex > borders.Count)
                {
                    activeIndex = borders.Count - 1;
                }
                return borderExists;
            }
        }

        public BorderPropObject ActiveBorder
        { 
            get
            {
                return borders[activeIndex];
            }
        }

        public int BorderWidthInPixels
        {
            get
            {
                return (int)(ActiveBorder.width * borderScale);
            }
        }

        public int BorderHeightInPixels
        {
            get
            {
                if (ActiveBorder.uniformSize) return BorderWidthInPixels;
                else return (int)(ActiveBorder.height * borderScale);
            }
        }

        public static System.Action OnBorderEnabled;
        public static System.Action<BorderPropObject> OnBorderUpdated;
        public static System.Action OnBorderDisabled;

        int cachedBorderWidth = 0;
        int cachedBorderHeight = 0;

        public static Texture2D borderTexture = null;
        static GUIStyle borderStyle = null;

        /// <summary>
        /// True if the current border's width and height add up to 0, or if the color's alpha is 0
        /// </summary>
        bool activeBorderIsEmpty = false;

        const string DEBUG_TITLE = "Sinden Border: ";

        [HideInInspector] Canvas parentCanvas = null;

        private void OnValidate()
        {
            bool foundThisFrame = false;
            if (parentCanvas == null)
            {
                parentCanvas = transform.root.GetComponentInChildren<Canvas>();
                foundThisFrame = true;
            }
            if (!foundThisFrame)
            {
                if (transform.root.GetComponentInChildren<Canvas>() != parentCanvas)
                {
                    parentCanvas = transform.root.GetComponentInChildren<Canvas>();
                }
            }
        }

        private void OnEnable()
        {
            if (HasBorders)
            {
                ForceUpdateBorder();
            }
            else
            {
                Debug.LogWarning(DEBUG_TITLE + "No border objects found in SindenBorder component attached to " + gameObject.name + "! Disabling...");
                enabled = false;
            }
            OnBorderEnabled?.Invoke();
        }

        private void OnDisable()
        {
            OnBorderDisabled?.Invoke();
        }

        /// <summary>
        /// Draws the border if this component is enabled
        /// </summary>
        private void OnGUI()
        {
            if (activeBorderIsEmpty) return;
            // Draw border
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, borderStyle);
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;
            if (activeBorderIsEmpty) return;
            if (borderTexture == null) ForceUpdateBorder();
            Graphics.DrawTexture(new Rect(0, 0, parentCanvas.pixelRect.width, parentCanvas.pixelRect.height), borderTexture);
        }

        /// <summary>
        /// Will call ForceUpdateBorder during runtime
        /// </summary>
        [ContextMenu("Apply Border Properties")]
        public void ApplyBorderProperties()
        {
            if (borders[activeIndex] != null)
            {
                // Set RectTransform scaling mode to Stretch
                RectTransform rect = transform as RectTransform;
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);

                BorderPropObject border = ActiveBorder;

                activeBorderIsEmpty = border.width + border.height == 0 || border.width == 0 && border.uniformSize || border.color.a == 0;

                cachedBorderWidth = Mathf.Abs(BorderWidthInPixels);
                cachedBorderHeight = Mathf.Abs(BorderHeightInPixels);

                rect.sizeDelta = new Vector2(cachedBorderWidth * -2, cachedBorderHeight * -2);
                if (!activeBorderIsEmpty)
                {
                    if (Application.isPlaying)
                    {
                        ForceUpdateBorder();
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (UnityEditor.Selection.activeGameObject == gameObject)
                        {
                            ForceUpdateBorder();
                        }
#endif
                    }
                }
                else
                {
                    OnBorderUpdated?.Invoke(ActiveBorder);
                }

                LogAction(DEBUG_TITLE + "Applied new border + " + borders[activeIndex].name);
            }
        }

        public void ApplyPreviousBorder()
        {
            RecordObjectWithUndo();
            activeIndex = (int)Mathf.Repeat(activeIndex - 1, borders.Count);

            ApplyBorderProperties();
        }

        public void ApplyNextBorder()
        {
            RecordObjectWithUndo();
            activeIndex = (int)Mathf.Repeat(activeIndex + 1, borders.Count);

            ApplyBorderProperties();
        }

        /// <summary>
        /// Recommended to call this when the game's screen size changes
        /// </summary>
        public void ForceUpdateBorder()
        {
            RectTransform parentRect = parentCanvas.transform as RectTransform;
            int screenWidth = (int)parentRect.sizeDelta.x;
            int screenHeight = (int)parentRect.sizeDelta.y;

            borderTexture = new Texture2D(screenWidth, screenHeight);
            cachedBorderWidth = BorderWidthInPixels;
            cachedBorderHeight = BorderHeightInPixels;

            // Texture is gray for some reason, clear it
            int arrayLength = screenWidth * screenHeight;
            Color[] transparentArray = new Color[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                transparentArray[i] = Color.clear;
            }
            borderTexture.SetPixels(transparentArray);

            // Draw top bar the of border
            for (int y = 0; y < cachedBorderHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    borderTexture.SetPixel(x, y, ActiveBorder.color);
                }
            }

            // Draw bottom bar the of border
            for (int y = screenHeight - cachedBorderHeight; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    borderTexture.SetPixel(x, y, ActiveBorder.color);
                }
            }

            // Draw left bar the of border
            for (int y = cachedBorderHeight; y < screenHeight - cachedBorderHeight; y++)
            {
                for (int x = 0; x < cachedBorderWidth; x++)
                {
                    borderTexture.SetPixel(x, y, ActiveBorder.color);
                }
            }

            // Draw right bar the of border
            for (int y = cachedBorderHeight; y < screenHeight - cachedBorderHeight; y++)
            {
                for (int x = screenWidth - cachedBorderWidth; x < screenWidth; x++)
                {
                    borderTexture.SetPixel(x, y, ActiveBorder.color);
                }
            }

            borderTexture.Apply();
            borderStyle = new GUIStyle();
            borderStyle.normal.background = borderTexture;

            OnBorderUpdated?.Invoke(ActiveBorder);
        }

        void RecordObjectWithUndo()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Applied a new Sinden Border");
#endif
        }

        void LogAction(string action)
        {
            if (logActions)
            {
                Debug.Log(action);
            }
        }
    }
}