using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SindenUnity
{
    [CustomEditor(typeof(BorderPropObject))]
    public class BorderPropEditor : Editor
    {
        SerializedProperty borderProperties;
        SerializedProperty width;
        SerializedProperty height;
        SerializedProperty uniformSize;
        SerializedProperty color;

        private void OnEnable()
        {
            borderProperties = serializedObject.FindProperty("borderProperties");
            width = borderProperties.FindPropertyRelative("widths");
            height = borderProperties.FindPropertyRelative("height");
            uniformSize = borderProperties.FindPropertyRelative("uniformSize");
            color = borderProperties.FindPropertyRelative("color");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(width);
            EditorGUILayout.PropertyField(height);
            EditorGUILayout.PropertyField(uniformSize);
            EditorGUILayout.PropertyField(color);

            if (GUILayout.Button(new GUIContent("Reset", "Resets border properties to their defaults")))
            {
                ((BorderPropObject)target).Reset();
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}