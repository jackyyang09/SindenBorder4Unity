using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SindenUnity
{
    [CustomEditor(typeof(BorderListener))]
    public class BorderListenerEditor : Editor
    {
        BorderListener myScript = null;

        private void OnEnable()
        {
            myScript = target as BorderListener;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!myScript.IsReady)
            {
                EditorGUILayout.HelpBox(
                    "Please set your RectTransform's anchor min to (0, 0) and anchor max to (1, 1) to use this component!", 
                    MessageType.Warning);
            }

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;

            EditorGUILayout.LabelField(
                "A BorderListener will listen for changes to the active Sinden Border and " +
                "will adjust it's RectTransform size accordingly, creating buffer room for all " +
                "RectTransforms parented to this GameObject.", labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "BorderListeners only work during runtime.", labelStyle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "BorderListeners work best parented to a Screen Space canvas.", labelStyle);

            EditorGUILayout.HelpBox(
                "Make sure that this RectTransform isn't the child of a RectTransform with a SindenBorder component. " +
                "Otherwise, unpredictable behaviour will occur.",
                MessageType.Info);

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        [MenuItem("GameObject/SindenBorder4Unity/BorderListener", false, 1)]
        public static void AddSindenBorder()
        {
            GameObject newObject = new GameObject("BorderListener");

            Canvas canvas = null;
            GameObject canvasParent = null;
            if (Selection.activeTransform != null)
            {
                canvas = Selection.activeTransform.GetComponentInParent<Canvas>();
            }

            if (!canvas)
            {
                canvas = FindObjectOfType<Canvas>();
            }

            if (canvas) canvasParent = canvas.gameObject;

            if (!canvas)
            {
                canvasParent = new GameObject("New Sinden Canvas");
                canvasParent.transform.SetParent(Selection.activeTransform);
                canvas = canvasParent.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasParent.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasParent.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Undo.RegisterCreatedObjectUndo(canvasParent, "Added new parent for BorderListener");
            }

            newObject.transform.localPosition = Vector3.zero;
            GameObjectUtility.SetParentAndAlign(newObject, canvasParent);

            RectTransform rect = newObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = Vector2.zero;
            newObject.AddComponent<BorderListener>();

            Selection.activeGameObject = newObject;
            Undo.RegisterCreatedObjectUndo(newObject, "Added new BorderListener");
        }
    }
}