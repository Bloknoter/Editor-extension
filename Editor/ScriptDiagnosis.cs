using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;


namespace EditorExtension.Diagnostics
{
    public class ScriptDiagnosis : EditorWindow
    {
        [MenuItem("Extension/Script Diagnosis")]
        private static void GetWindow()
        {
            GetWindow(typeof(ScriptDiagnosis), false, "Diagnosis", true).position = new Rect(500, 100, 500, 500);
        }

        private Vector2 scrollPos;

        private MonoBehaviour target;

        private bool showFullNames = false;

        private bool isDiagnosed;

        private Type type;

        private List<FieldInfo> publicFieldInfos = new List<FieldInfo>();
        private List<FieldInfo> nonpublicFieldInfos = new List<FieldInfo>();
        private List<FieldInfo> staticFieldInfos = new List<FieldInfo>();


        private List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

        private List<EventInfo> eventInfos = new List<EventInfo>();

        private void OnGUI()
        {
            MonoBehaviour newmono = (MonoBehaviour)EditorGUILayout.ObjectField(new GUIContent("Target script"), target, typeof(MonoBehaviour), true);
            showFullNames = EditorGUILayout.Toggle("Show full type names", showFullNames);
            if (GUILayout.Button("Diagnose", GUILayout.Width(120)) && target != null)
            {
                isDiagnosed = true;
                target = newmono;
                publicFieldInfos.Clear();
                nonpublicFieldInfos.Clear();
                staticFieldInfos.Clear();
                eventInfos.Clear();
                propertyInfos.Clear();

                type = target.GetType();

                List<FieldInfo> fieldInfos = new List<FieldInfo>();

                eventInfos.AddRange(type.GetRuntimeEvents());
                propertyInfos.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
                fieldInfos.AddRange(type.GetRuntimeFields());
                for (int i = 0; i < fieldInfos.Count; i++)
                {
                    //Debug.Log($"field: {fieldInfos[i].Name}    type: {fieldInfos[i].MemberType}");
                    bool isEvent = type.GetEvent(fieldInfos[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
                    /*if (type.GetProperty(fieldInfos[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
                        Debug.Log($"it is property");*/
                    if (!isEvent)
                    {
                        if (fieldInfos[i].IsStatic)
                        {
                            staticFieldInfos.Add(fieldInfos[i]);
                        }
                        else if (fieldInfos[i].IsPublic)
                        {
                            publicFieldInfos.Add(fieldInfos[i]);
                        }
                        else
                        {
                            nonpublicFieldInfos.Add(fieldInfos[i]);
                        }
                    }
                }
                for(int i = 0; i < propertyInfos.Count;i++)
                {
                    if(propertyInfos[i].DeclaringType == typeof(MonoBehaviour) || propertyInfos[i].DeclaringType == typeof(Behaviour) ||
                        propertyInfos[i].DeclaringType == typeof(Component) || propertyInfos[i].DeclaringType == typeof(UnityEngine.Object))
                    {
                        propertyInfos.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (newmono == target && target != null && isDiagnosed)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
                EditorGUILayout.BeginVertical(GUILayout.Width(3000));
                EditorGUILayout.LabelField($"Type: {ConvertTypeName(type)}");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("FIELDS");
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                if (publicFieldInfos.Count > 0)
                {
                    EditorGUILayout.LabelField("public: ");
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < publicFieldInfos.Count; i++)
                    {
                        DisplayField(publicFieldInfos[i]);
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                if (nonpublicFieldInfos.Count > 0)
                {
                    EditorGUILayout.LabelField("non-public: ");
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < nonpublicFieldInfos.Count; i++)
                    {
                        DisplayField(nonpublicFieldInfos[i]);
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                if (staticFieldInfos.Count > 0)
                {
                    EditorGUILayout.LabelField("static: ");
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < staticFieldInfos.Count; i++)
                    {
                        DisplayField(staticFieldInfos[i]);
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                }
                if (publicFieldInfos.Count == 0 && nonpublicFieldInfos.Count == 0 && staticFieldInfos.Count == 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("No fields found");
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

                if (propertyInfos.Count > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("PROPERTIES");
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < propertyInfos.Count; i++)
                    {
                        DisplayProperty(propertyInfos[i]);
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                if (eventInfos.Count > 0)
                {
                    EditorGUILayout.LabelField("EVENTS");
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < eventInfos.Count; i++)
                    {
                        DisplayEvent(eventInfos[i]);
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                target = newmono;
                isDiagnosed = false;
            }
        }

        private void DisplayField(FieldInfo field)
        {
            object value = field.GetValue(target);
            if (value is IList)
            {
                EditorGUILayout.LabelField($"{field.Name} ({ConvertTypeName(field.FieldType)}) : ");
                EditorGUI.indentLevel++;
                DisplayList((IList)value);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField($"{field.Name} ({ConvertTypeName(field.FieldType)}) : {ParseValue(value)}");
            }
        }

        private void DisplayEvent(EventInfo eventInfo)
        {
            MulticastDelegate multicastDelegate = (MulticastDelegate)target.GetType().GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(target);
            if (multicastDelegate != null)
            {
                EditorGUILayout.LabelField($"Event : {eventInfo.Name}");
                Delegate[] delegates = multicastDelegate.GetInvocationList();
                if (delegates.Length > 0)
                {
                    EditorGUI.indentLevel++;
                    for (int m = 0; m < delegates.Length; m++)
                    {
                        EditorGUILayout.LabelField($"listener: {delegates[m].Method.DeclaringType.FullName}.{delegates[m].Method.Name}");
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("No listeners defined");
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("null event");
                EditorGUI.indentLevel--;
            }
        }

        private void DisplayProperty(PropertyInfo property)
        {
            string getter= "";
            if (property.CanRead)
            {
                if (property.GetMethod.IsPublic)
                {
                    getter = "public";
                }
                else if (property.GetMethod.IsPrivate)
                {
                    getter = "private";
                }
                else
                {
                    getter = "protected";
                }
                if (property.GetMethod.IsStatic)
                    getter += " static";
                getter += " get; ";
            }
            string setter = "";
            if (property.CanWrite)
            {
                if (property.SetMethod.IsPublic)
                {
                    setter = "public";
                }
                else if (property.SetMethod.IsPrivate)
                {
                    setter = "private";
                }
                else
                {
                    setter = "protected";
                }
                if (property.SetMethod.IsStatic)
                    setter += " static";
                setter += " set; ";
            }
            if (property.CanRead)
            {
                object value = property.GetValue(target);
                if (value is IList)
                {
                    EditorGUILayout.LabelField($"{property.Name} ({ConvertTypeName(property.PropertyType)}) {{ {getter}{setter}}} : ");
                    EditorGUI.indentLevel++;
                    DisplayList((IList)value);
                    EditorGUI.indentLevel--;
                }
                else
                    EditorGUILayout.LabelField($"{property.Name} ({ConvertTypeName(property.PropertyType)}) {{ {getter}{setter}}} : {ParseValue(value)}");
            }
            else
                EditorGUILayout.LabelField($"{property.Name} {{ {setter}}}");
        }

        private void DisplayList(IList list)
        {
            if (list != null)
            {
                EditorGUILayout.LabelField($"size: {list.Count}");
                if (list.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    IEnumerator enumerator = list.GetEnumerator();
                    enumerator.MoveNext();
                    for (int i = 0; i < list.Count; i++)
                    {
                        object value = enumerator.Current;
                        EditorGUILayout.LabelField($"[{i}] : {ParseValue(value)}");
                        enumerator.MoveNext();
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
                EditorGUILayout.LabelField($"null array");
        }

        private string ConvertTypeName(Type type)
        {
            if (type == null)
                return "null type";
            if (showFullNames)
                return type.ToString();
            if(type == typeof(int))
            {
                return "int";
            }
            if (type == typeof(bool))
            {
                return "bool";
            }
            if (type == typeof(byte))
            {
                return "byte";
            }
            if (type == typeof(long))
            {
                return "long";
            }
            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(double))
            {
                return "double";
            }
            if (type == typeof(string))
            {
                return "string";
            }
            if(type.IsEnum)
            {
                return "enum";
            }
            if(type == typeof(Vector2))
            {
                return "Vector2";
            }
            if (type == typeof(Vector3))
            {
                return "Vector3";
            }
            if (type == typeof(Color))
            {
                return "Color";
            }
            if (type.IsArray)
            {
                return "Array";
            }
            else if(type.GetInterface("IList") != null)
            {
                return "List";
            }
            else if(type.GetInterface("IDictionary") != null)
            {
                return "Dictionary";
            }
            string typeStr = type.ToString();
            string result = "";
            for(int i = 0; i < typeStr.Length;i++)
            {
                if(typeStr[i] == '.')
                {
                    if(i < typeStr.Length - 1)
                    {
                        result = "";
                    }
                }
                else
                {
                    result += typeStr[i];
                }
            }
            return result;
        }

        private string ParseValue(object value)
        {
            if (value != null)
            {
                if (value is Vector2)
                    return $"[ {FormatFloats(((Vector2)value).x)} , {FormatFloats(((Vector2)value).y)} ]";
                if (value is Vector3)
                    return $"[ {FormatFloats(((Vector3)value).x)} , {FormatFloats(((Vector3)value).y)} , {FormatFloats(((Vector3)value).z)} ]";
                if (value is Color)
                    return $"[ {FormatFloats(((Color)value).r)} , {FormatFloats(((Color)value).g)} , {FormatFloats(((Color)value).b)} , a : {FormatFloats(((Color)value).a)} ]";
                string valueStr = value.ToString();
                return valueStr;
            }
            else
                return "null";
        }

        private string FormatFloats(float value)
        {
            return string.Format("{0:0.00000}", value);
        }
    }
}
