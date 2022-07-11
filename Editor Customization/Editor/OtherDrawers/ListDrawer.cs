using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using EditorExtension.CuriousInspector.AttributeHandlers;


namespace EditorExtension.Properties
{
    public class ListDrawer
    {
        public static void DrawList(SerializedProperty property, DrawSettings drawSettings)
        {
            DrawList(property, drawSettings.label.text);
        }

        public static void DrawList(SerializedProperty property, string name)
        {
            GUILayout.BeginVertical("Tooltip");

            GUIContent expandercontent = new GUIContent(name);
            if (property.isExpanded)
                expandercontent.image = EditorGUIUtility.IconContent("IN foldout act on").image;
            else
                expandercontent.image = EditorGUIUtility.IconContent("IN foldout").image;
            expandercontent.text += " (Array)";
            TextAnchor textAnchor = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(expandercontent))
            {
                property.isExpanded = !property.isExpanded;
            }
            GUI.skin.button.alignment = textAnchor;
            if (property.isExpanded)
            {


                EditorGUI.indentLevel += 1;
                for (int i = 0; i < property.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        property.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 1;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    if (property.arraySize > 0)
                        property.InsertArrayElementAtIndex(property.arraySize - 1);
                    else
                        property.InsertArrayElementAtIndex(0);
                }
                EditorGUILayout.LabelField("to this array");
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
