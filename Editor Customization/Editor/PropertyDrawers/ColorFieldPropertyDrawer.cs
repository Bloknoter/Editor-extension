using UnityEngine;
using UnityEditor;
using System;


namespace EditorExtension.Properties
{
    [CustomPropertyDrawer(typeof(Color))]
    public class ColorFieldPropertyDrawer : PropertyDrawer
    {
        private const float BUTTONS_WIDTH = 50;
        private const float BUTTONS_SPACING = 5;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width < 2 * BUTTONS_WIDTH + 2 * BUTTONS_SPACING + 200)
            {
                property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
            }
            else
            {
                property.colorValue = EditorGUI.ColorField(new Rect(position.x, position.y, position.width - (BUTTONS_WIDTH * 2) - (BUTTONS_SPACING * 2), position.height),
                    label, property.colorValue);
                Color previous = GUI.backgroundColor;
                GUI.backgroundColor = property.colorValue;
                if (GUI.Button(new Rect(position.x + position.width - (BUTTONS_WIDTH * 2) - BUTTONS_SPACING, position.y, BUTTONS_WIDTH, position.height), "Copy"))
                {
                    object[] color = new object[]{ property.colorValue.r, property.colorValue.g, property.colorValue.b,
                    property.colorValue.a};
                    ClipboardUtility.WriteData(color);
                }
                GUI.backgroundColor = Color.white;
                if (GUI.Button(new Rect(position.x + position.width - BUTTONS_WIDTH, position.y, BUTTONS_WIDTH, position.height), "Paste"))
                {
                    string[] stringcolor = ClipboardUtility.ReadData();
                    bool canparse = true;
                    if (stringcolor.Length < 4)
                    {
                        canparse = false;
                    }
                    if (canparse)
                    {
                        try
                        {
                            Color color = new Color(float.Parse(stringcolor[0]), float.Parse(stringcolor[1]), float.Parse(stringcolor[2]),
                                float.Parse(stringcolor[3]));
                            property.colorValue = color;
                        }
                        catch (Exception) { }
                    }
                }
                GUI.backgroundColor = previous;
            }
        }
    }
}
