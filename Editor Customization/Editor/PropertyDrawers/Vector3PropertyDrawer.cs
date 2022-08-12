using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace EditorExtension.Properties
{
    [CustomPropertyDrawer(typeof(Vector3))]
    public class Vector3PropertyDrawer : PropertyDrawer
    {
        private const float BUTTONS_WIDTH = 20;
        private const float BUTTONS_SPACING = 7;
        private const float SPACING = 5;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width < 2 * BUTTONS_WIDTH + 2 * BUTTONS_SPACING + 235)
            {
                property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
            }
            else
            {
                float previouswidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth /= 2.0f;
                property.vector3Value = EditorGUI.Vector3Field(new Rect(position.x, position.y, position.width - (BUTTONS_WIDTH * 2) - (BUTTONS_SPACING * 2), position.height),
                    label, property.vector3Value);
                EditorGUIUtility.labelWidth = previouswidth;

                Color previous = GUI.backgroundColor;
                GUI.backgroundColor = Color.white;

                if (GUI.Button(new Rect(position.x + position.width - (BUTTONS_WIDTH * 2) - BUTTONS_SPACING, position.y, BUTTONS_WIDTH, position.height), "C"))
                {
                    object[] vector = new object[] { property.vector3Value.x, property.vector3Value.y, property.vector3Value.z };
                    ClipboardUtility.WriteData(vector);
                }
                if (GUI.Button(new Rect(position.x + position.width - BUTTONS_WIDTH, position.y, BUTTONS_WIDTH, position.height), "P"))
                {
                    string[] stringvector = ClipboardUtility.ReadData();
                    bool canparse = true;
                    if (stringvector.Length < 3)
                    {
                        canparse = false;
                    }
                    if (canparse)
                    {
                        try
                        {
                            Vector3 vector = new Vector3(float.Parse(stringvector[0]), float.Parse(stringvector[1]), float.Parse(stringvector[2]));
                            property.vector3Value = vector;
                        }
                        catch (Exception) { }
                    }
                }
                GUI.backgroundColor = previous;
            }
        }
    }
}
