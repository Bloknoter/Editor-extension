using UnityEngine;
using UnityEditor;
using System;


namespace EditorExtension.Properties
{
    [CustomPropertyDrawer(typeof(Vector2))]
    public class Vector2PropertyDrawer : PropertyDrawer
    {
        private const float LABEL_WIDTH = 150f;
        private const float BUTTONS_WIDTH = 35;
        private const float FIELDS_WIDTH = 70f;
        private const float SPACING = 5;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.width < LABEL_WIDTH + FIELDS_WIDTH + SPACING + FIELDS_WIDTH - 100f)
            {
                property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
            }
            else
            {
                EditorGUI.LabelField(new Rect(position.x, position.y, LABEL_WIDTH, position.height), label);
                float previouswidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 10f;
                /*property.vector2Value = EditorGUI.Vector2Field(new Rect(position.x, position.y, position.width - (BUTTONS_WIDTH * 2) - (BUTTONS_SPACING * 2), position.height),
                    label, property.vector2Value);*/
                //EditorGUI.LabelField(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 15f, position.height), "X:");
                float x = EditorGUI.FloatField(new Rect(position.x + LABEL_WIDTH, position.y, FIELDS_WIDTH, position.height), "X", property.vector2Value.x);
                //EditorGUI.LabelField(new Rect(position.x + EditorGUIUtility.labelWidth + 17f, position.y, 15f, position.height), "Y:");
                float y = EditorGUI.FloatField(new Rect(position.x + LABEL_WIDTH + FIELDS_WIDTH + SPACING, position.y, FIELDS_WIDTH, position.height), "Y", property.vector2Value.y);
                property.vector2Value = new Vector2(x, y);
                EditorGUIUtility.labelWidth = previouswidth;

                if (position.width >= 2 * BUTTONS_WIDTH + 2 * SPACING + 230)
                {
                    Color previous = GUI.backgroundColor;
                    GUI.backgroundColor = Color.white;

                    if (GUI.Button(new Rect(position.x + LABEL_WIDTH + FIELDS_WIDTH + SPACING + FIELDS_WIDTH + SPACING, position.y, BUTTONS_WIDTH, position.height), "C"))
                    {
                        object[] vector = new object[] { property.vector2Value.x, property.vector2Value.y };
                        ClipboardUtility.WriteData(vector);
                    }
                    if (GUI.Button(new Rect(position.x + LABEL_WIDTH + FIELDS_WIDTH + SPACING + FIELDS_WIDTH + SPACING + BUTTONS_WIDTH + SPACING, position.y, BUTTONS_WIDTH, position.height), "P"))
                    {
                        string[] stringvector = ClipboardUtility.ReadData();
                        bool canparse = true;
                        if (stringvector.Length < 2)
                        {
                            canparse = false;
                        }
                        if (canparse)
                        {
                            try
                            {
                                Vector2 vector = new Vector2(float.Parse(stringvector[0]), float.Parse(stringvector[1]));
                                property.vector2Value = vector;
                            }
                            catch (Exception) { }
                        }
                    }
                    GUI.backgroundColor = previous;
                }
            }
        }
    }
}
