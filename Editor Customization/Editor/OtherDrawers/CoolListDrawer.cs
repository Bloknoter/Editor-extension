using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EditorExtension.Attributes.CustomAttributes;


namespace EditorExtension.Properties
{
    [CustomPropertyDrawer(typeof(CoolList))]
    public class CoolListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string path = property.propertyPath;
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.')
                {
                    path = path.Remove(i, 1);
                    break;
                }
                else
                {
                    path = path.Remove(i, 1);
                }
            }
            property = property.serializedObject.FindProperty(path);
            if (!property.isArray)
            {
                EditorGUI.LabelField(position, "CoolList cannot be applied to non-array type");
                return;
            }
            float ypos = position.y;
            GUILayout.BeginVertical("Tooltip");

            if (property.isExpanded)
                label.image = EditorGUIUtility.IconContent("IN foldout act on").image;
            else
                label.image = EditorGUIUtility.IconContent("IN foldout").image;
            label.text += " (Array)";
            TextAnchor textAnchor = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            if (GUILayout.Button(label))
            {
                property.isExpanded = !property.isExpanded;
            }
            GUI.skin.button.alignment = textAnchor;
            ypos += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isArray)
            {
                return height;
            }
            if (property.isExpanded)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                }
            }
            return height;
        }
    }
}
