using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SindenUnity
{
    [CustomEditor(typeof(BorderPropObject))]
    public class BorderPropEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, new string[]{ "m_Script" });

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