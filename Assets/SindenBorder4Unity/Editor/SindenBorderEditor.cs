using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SindenUnity
{
    [CustomEditor(typeof(SindenBorder))]
    public class SindenBorderEditor : Editor
    {
        SindenBorder myScript = null;

        SerializedProperty borders = null;

        private void OnEnable()
        {
            myScript = (SindenBorder)target;

            borders = serializedObject.FindProperty("borders");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;
            EditorGUILayout.LabelField("Note: Gizmos need to be enabled to see the border in the Scene View", labelStyle);
            EditorGUILayout.LabelField("Border shows up in the Game View only during runtime.", labelStyle);

            EditorGUILayout.Space();

            if (myScript.transform.parent == null)
            {
                EditorGUILayout.HelpBox(
                    "This GameObject must be a child of a Canvas " +
                    "in order to function properly!", MessageType.Error);
            }

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

            using (new EditorGUI.DisabledScope(myScript.Borders.Count == 0))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(new GUIContent("▲", "Apply the previous border in the border preset list")))
                {
                    myScript.ApplyPreviousBorder();
                }

                if (GUILayout.Button(new GUIContent("▼", "Apply the next border in the border preset list")))
                {
                    myScript.ApplyNextBorder();
                }

                if (GUILayout.Button(new GUIContent("Apply Border Properties", "Apply the properties of the border to this RectTransform")))
                {
                    myScript.ApplyBorderProperties();
                }

                if (GUILayout.Button(new GUIContent("Add New", "Create a new Border Properties Object and add it to the Border list")))
                {
                    var asset = OpenSmartSaveFileDialog<BorderPropObject>(false, "New Border Properties");
                    if (asset != null)
                    {
                        borders.InsertArrayElementAtIndex(borders.arraySize);
                        borders.GetArrayElementAtIndex(borders.arraySize - 1).objectReferenceValue = asset;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public static void CreateAssetSafe(Object asset, string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                Debug.LogError("Error! Attempted to write an asset over a folder!");
                return;
            }
            
            AssetDatabase.CreateAsset(asset, path);
        }

        public static T OpenSmartSaveFileDialog<T>(bool highlightAfterCreation = true, string defaultName = "New Object", string startingPath = "Assets") where T : ScriptableObject
        {
            string savePath = EditorUtility.SaveFilePanel("Designate save path", startingPath, defaultName, "asset");
            if (savePath != "") // Make sure user didn't press "Cancel"
            {
                var asset = CreateInstance<T>();
                savePath = savePath.Remove(0, savePath.IndexOf("Assets/"));
                CreateAssetSafe(asset, savePath);
                if (highlightAfterCreation)
                {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = asset;
                }
                else
                {
                    EditorGUIUtility.PingObject(asset);
                }

                return asset;
            }

            return null;
        }

        [MenuItem("GameObject/SindenBorder4Unity/SindenBorder", false, 10)]
        public static void AddSindenBorder()
        {
            SindenBorder existingObject = FindObjectOfType<SindenBorder>();
            if (!existingObject)
            {
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
                    UnityEngine.UI.CanvasScaler scaler = canvasParent.AddComponent<UnityEngine.UI.CanvasScaler>();
                    scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasParent.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    Undo.RegisterCreatedObjectUndo(canvasParent, "Added new parent for BorderListener");
                }

                GameObject temp = new GameObject();
                string path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(temp.AddComponent<BorderListener>()));
                DestroyImmediate(temp);
                path = path.Remove(path.IndexOf("Scripts/BorderListener.cs"));
                path += "Prefabs/SindenBorder.prefab";
                GameObject newObject = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)));
                newObject.name = newObject.name.Remove(newObject.name.IndexOf("(Clone)"));
                GameObjectUtility.SetParentAndAlign(newObject, canvasParent);
                RectTransform rect = newObject.transform as RectTransform;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;

                SerializedObject so = new SerializedObject(newObject.GetComponent<SindenBorder>());
                so.FindProperty("parentCanvas").objectReferenceValue = canvas;
                so.ApplyModifiedProperties();

                Selection.activeGameObject = newObject;
                Undo.RegisterCreatedObjectUndo(newObject, "Added new SindenBorder");
            }
            else
            {
                Selection.activeGameObject = existingObject.gameObject;
                EditorGUIUtility.PingObject(existingObject);
                Debug.Log("SindenBorder already exists in this scene!");
            }
        }
    }
}