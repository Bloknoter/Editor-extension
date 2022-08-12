using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using EditorExtension.CuriousInspector.AttributeHandlers;
using CuriousAttributes;
using EditorExtension.CuriousInspector.Settings;
using EditorExtension.Properties;

namespace EditorExtension.CuriousInspector
{
    public class DrawUtility
    {
        private class DefaultSettings
        {
            private static Color backgroundColor;
            private static Color textColor;
            public static void Read()
            {
                backgroundColor = GUI.backgroundColor;
                textColor = EditorStyles.label.normal.textColor;
            }
            public static void Set()
            {
                GUI.backgroundColor = backgroundColor;
                GUI.contentColor = SettingsData.GetSettingsData().TextLabelColor;
                EditorStyles.label.normal.textColor = SettingsData.GetSettingsData().TextLabelColor;
                EditorStyles.miniButton.normal.textColor = SettingsData.GetSettingsData().TextLabelColor;
            }
        }

        private static void Log(string message)
        {
            if (SettingsData.GetSettingsData().DrawDebug)
            {
                Debug.Log(message);
            }
        }

        #region Drawing functions
        public static void PropertyField(SerializedProperty serializedProperty, FieldInfo fieldInfo)
        {
            DefaultSettings.Read();
            DefaultSettings.Set();
            PropertyField(serializedProperty, fieldInfo, null);
        }

        public static void PropertyField(SerializedProperty serializedProperty, FieldInfo fieldInfo, DrawSettings drawSettings)
        {

            if (drawSettings != null)
            {
                if (!drawSettings.display) return;
                Log($"drawing: {fieldInfo.Name}, type: {serializedProperty.type}, drawingSettings: \n " + drawSettings.ToString());
                DefaultSettings.Read();
                EditorStyles.label.normal.textColor = SettingsData.GetSettingsData().TextLabelColor;
                GUI.contentColor = SettingsData.GetSettingsData().TextLabelColor;
                GUI.backgroundColor = SettingsData.GetSettingsData().TextLabelColor;
                /*EditorStyles.label.normal.textColor = drawSettings.textColor;
                GUI.contentColor = drawSettings.textColor;
                GUI.backgroundColor = drawSettings.backgroundColor;*/
            }
            else
            {
                Log("Creating new draw settings");
                drawSettings = new DrawSettings();
                Log("created draw settings: \n" + drawSettings.ToString());
            }

            if (drawSettings.label.text == "")
                drawSettings.label.text = fieldInfo.Name;
            if (fieldInfo.GetValue(serializedProperty.serializedObject.targetObject) is IList/*serializedProperty.isArray*/)
            {
                Log($"{fieldInfo.Name} is List type, drawing it as list");
                ListDrawer.DrawList(serializedProperty, drawSettings);
            }
            else
            {
                if (!drawSettings.displayonly)
                {
                    /*Rect rect = new Rect(EditorGUILayout.GetControlRect());
                    rect.height = EditorGUI.GetPropertyHeight(serializedProperty);
                    EditorGUI.PropertyField(rect, serializedProperty, new GUIContent(drawSettings.label.text, drawSettings.label.image));*/
                    EditorGUILayout.PropertyField(serializedProperty, new GUIContent(drawSettings.label.text, drawSettings.label.image));
                }
                else
                {
                    object value = fieldInfo.GetValue(serializedProperty.serializedObject.targetObject);
                    if (value != null)
                        EditorGUILayout.LabelField(drawSettings.label, new GUIContent(value.ToString()));
                    else
                        EditorGUILayout.LabelField(drawSettings.label, new GUIContent("null"));
                }
            }
            DefaultSettings.Set();
        }

        public static bool Button(DrawSettings drawSettings)
        {
            DefaultSettings.Read();
            GUI.backgroundColor = drawSettings.backgroundColor;
            bool result = GUI.Button(EditorGUILayout.GetControlRect(), drawSettings.label.text);

            DefaultSettings.Set();
            return result;
        }

        #endregion

        #region Reflection functions

        public static IHandler[] GetHandlersfromAttributesofTypeandInitIt<T>(MemberInfo memberInfo) where T : MyAttribute
        {
            T[] attributes = (T[])memberInfo.GetCustomAttributes<T>();
            IHandler[] handlers = new IHandler[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                handlers[i] = attributes[i].GetHandler();
                handlers[i].Initialize(attributes[i]);
            }
            return handlers;
        }

        #endregion
    }
}
