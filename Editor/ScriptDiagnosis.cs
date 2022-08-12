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

        // GUI properties

        private Vector2 firstTabScrollPos;
        private Vector2 secondTabScrollPos;

        private string[] tabs = new string[] { "Parsing", "Invokation" };
        private int currtab;

        private bool showFullNames = false;

        private bool isDiagnosed;

        private bool invokableFound;

        // General target info

        private UnityEngine.Object target;

        private Type type;

        // Target info

        private List<FieldInfo> fieldInfos = new List<FieldInfo>();

        private List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
        private List<EventInfo> eventInfos = new List<EventInfo>();

        private List<MethodInfo> methodInfos = new List<MethodInfo>();

        // Labels

        private GUIContent errorIcon;

        private void OnEnable()
        {
            errorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");
            errorIcon.tooltip = "fix errors!";
        }

        private void OnGUI()
        {
            UnityEngine.Object newTarget = EditorGUILayout.ObjectField(new GUIContent("Target object"), target, typeof(UnityEngine.Object), true);
            showFullNames = EditorGUILayout.Toggle("Show full type names", showFullNames);

            EditorGUILayout.Separator();

            currtab = GUILayout.Toolbar(currtab, tabs);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (currtab == 0)
            {
                if (target != null)
                {
                    if (GUILayout.Button("Diagnose", GUILayout.Width(120)))
                    {
                        isDiagnosed = true;
                        target = newTarget;
                        fieldInfos.Clear();
                        propertyInfos.Clear();

                        ExtractType();

                        List<FieldInfo> newFieldInfos = new List<FieldInfo>();

                        eventInfos.AddRange(type.GetRuntimeEvents());
                        propertyInfos.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
                        newFieldInfos.AddRange(type.GetRuntimeFields());
                        for (int i = 0; i < newFieldInfos.Count; i++)
                        {
                            if (!isObsolete(newFieldInfos[i]))
                            {
                                //Debug.Log($"field: {fieldInfos[i].Name}    type: {fieldInfos[i].MemberType}");
                                bool isEvent = type.GetEvent(newFieldInfos[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
                                /*if (type.GetProperty(fieldInfos[i].Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null)
                                    Debug.Log($"it is property");*/
                                if (!isEvent)
                                {
                                    fieldInfos.Add(newFieldInfos[i]);
                                }
                            }
                        }
                        for (int i = 0; i < propertyInfos.Count; i++)
                        {
                            if (propertyInfos[i].DeclaringType == typeof(MonoBehaviour) || propertyInfos[i].DeclaringType == typeof(Behaviour) ||
                                propertyInfos[i].DeclaringType == typeof(Component) || propertyInfos[i].DeclaringType == typeof(UnityEngine.Object))
                            {
                                propertyInfos.RemoveAt(i);
                                i--;
                            }
                            else if(isObsolete(propertyInfos[i]))
                            {
                                propertyInfos.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    if (target != null && isDiagnosed)
                    {
                        firstTabScrollPos = EditorGUILayout.BeginScrollView(firstTabScrollPos, true, true);
                        EditorGUILayout.BeginVertical(GUILayout.Width(3000));
                        EditorGUILayout.LabelField($"Type: {ConvertTypeName(type)}");

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("FIELDS");
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel++;
                        if (fieldInfos.Count > 0)
                        {
                            for (int i = 0; i < fieldInfos.Count; i++)
                            {
                                DisplayField(fieldInfos[i]);
                                EditorGUILayout.Space();
                            }
                        }
                        else
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
                }
            }
            else if(currtab == 1)
            {
                if (target != null)
                {
                    if (GUILayout.Button("Search", GUILayout.Width(110)))
                    {
                        methodInfos.Clear();
                        target = newTarget;
                        ExtractType();

                        List<MethodInfo> newMethodInfos = new List<MethodInfo>();
                        newMethodInfos.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));

                        for(int i = 0; i < newMethodInfos.Count;i++)
                        {
                            if(!newMethodInfos[i].IsConstructor && !newMethodInfos[i].IsGenericMethod)
                            {
                                if(!isObsolete(newMethodInfos[i]))
                                {
                                    methodInfos.Add(newMethodInfos[i]);
                                }
                            }
                        }

                        methodInfos.AddRange(newMethodInfos);

                        invokableFound = true;
                    }
                }

                if(target != null && invokableFound)
                {
                    secondTabScrollPos = EditorGUILayout.BeginScrollView(secondTabScrollPos, true, true);
                    EditorGUILayout.BeginVertical(GUILayout.Width(3000));
                    EditorGUI.indentLevel++;
                    for(int i = 0; i < methodInfos.Count;i++)
                    {
                        DisplayMethodHeader(methodInfos[i]);
                        DisplayMethodParameters(methodInfos[i]);
                        EditorGUILayout.Separator();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                }
            }
            if(target == null)
            {
                EditorGUILayout.LabelField("Select target object to diagnose it");
            }
            if (target != newTarget || target == null)
            {
                target = newTarget;
                isDiagnosed = false;
                invokableFound = false;
            }
        }

        private void ExtractType()
        {
            if(target != null)
            {
                type = target.GetType();
            }
            else
            {
                type = null;
            }
        }

        private void DisplayField(FieldInfo field)
        {
            string mods = "";
            if (field.IsPublic)
                mods = "public ";
            else
                mods = "non-public ";
            if (field.IsStatic)
                mods += "static";
            object value = field.GetValue(target);
            if (value is IList)
            {
                EditorGUILayout.LabelField($"{mods} {field.Name} ({ConvertTypeName(field.FieldType)}) : ");
                EditorGUI.indentLevel++;
                DisplayListContent((IList)value);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField($"{mods} {field.Name} ({ConvertTypeName(field.FieldType)}) : {ParseValue(value)}");
            }
        }

        private void DisplayEvent(EventInfo eventInfo)
        {
            FieldInfo field = target.GetType().GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            MulticastDelegate multicastDelegate = (MulticastDelegate)field.GetValue(target);
            if (multicastDelegate != null)
            {
                string header = $"";
                if (field.IsPublic)
                    header = "public ";
                else
                    header = "non-public ";
                if (field.IsStatic)
                    header += "static ";
                header += $"event : {eventInfo.Name}";
                EditorGUILayout.LabelField(header);
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
                    DisplayListContent((IList)value);
                    EditorGUI.indentLevel--;
                }
                else
                    EditorGUILayout.LabelField($"{property.Name} ({ConvertTypeName(property.PropertyType)}) {{ {getter}{setter}}} : {ParseValue(value)}");
            }
            else
                EditorGUILayout.LabelField($"{property.Name} {{ {setter}}}");
        }

        private void DisplayListContent(IList list)
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

        private void DisplayMethodHeader(MethodInfo methodInfo)
        {
            string mods = "";
            if (methodInfo.IsPublic)
                mods = "public ";
            else
                mods = "non-public ";
            if (methodInfo.IsStatic)
                mods += "static";
            if (methodInfo.IsAbstract)
                mods += "abstract";
            if (methodInfo.IsVirtual)
                mods += "virtual";

            string returnType = ConvertTypeName(methodInfo.ReturnType);

            EditorGUILayout.LabelField($"{mods} {returnType} {methodInfo.Name}");
        }

        private void DisplayMethodParameters(MethodInfo methodInfo)
        {
            EditorGUI.indentLevel++;
            ParameterInfo[] parametersInfo = methodInfo.GetParameters();
            object[] parameters = new object[0];
            bool canBeInvoked = true;
            if (parametersInfo.Length > 0)
            {
                canBeInvoked = false;
                /*EditorGUILayout.LabelField("PARAMETERS: ");
                EditorGUILayout.Separator();
                
                parameters = new object[parametersInfo.Length];
                for (int i = 0; i < parametersInfo.Length; i++)
                {
                    if (parametersInfo[i].IsOut)
                    {
                        canBeInvoked = false;
                    }
                    else if (!parametersInfo[i].IsOptional)
                    {
                        if (parametersInfo[i].ParameterType == typeof(Color))
                        {
                            parameters[i] = EditorGUILayout.ColorField(parametersInfo[i].Name, pa)
                        }
                    }
                }*/
            }
            // Color AnimationCurve double float gradient int string long object rect rectint bool Vector2 Vetcor2int Vector3 Vector3INt Vector4
            if (canBeInvoked)
            {
                if (GUILayout.Button("Invoke", GUILayout.Width(100)))
                {
                    methodInfo.Invoke(target, parameters);
                }
            }
            else
            {
                GUIContent errorInfo = new GUIContent("Cannot invoke method because not all parameters could be set", errorIcon.image, "Invokation error");
                EditorGUILayout.LabelField(errorInfo);
            }
            EditorGUI.indentLevel--;
        }

        private bool isObsolete(FieldInfo memberInfo)
        {
            List<Attribute> attributes = new List<Attribute>();
            attributes.AddRange(memberInfo.GetCustomAttributes<Attribute>());
            for(int i = 0; i < attributes.Count;i++)
            {
                if (attributes[i].GetType() == typeof(ObsoleteAttribute))
                    return true;
            }
            return false;
        }

        private bool isObsolete(PropertyInfo memberInfo)
        {
            List<Attribute> attributes = new List<Attribute>();
            attributes.AddRange(memberInfo.GetCustomAttributes<Attribute>());
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].GetType() == typeof(ObsoleteAttribute))
                    return true;
            }
            return false;
        }

        private bool isObsolete(MethodInfo memberInfo)
        {
            List<Attribute> attributes = new List<Attribute>();
            attributes.AddRange(memberInfo.GetCustomAttributes<Attribute>());
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].GetType() == typeof(ObsoleteAttribute))
                    return true;
            }
            return false;
        }

        private string ConvertTypeName(Type type)
        {
            if (type == null)
                return "null type";
            if (type == typeof(void))
                return "void";
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
                    return $"[ {FormatFloats(((Color)value).r)} , {FormatFloats(((Color)value).g)} , {FormatFloats(((Color)value).b)} , alpha : {FormatFloats(((Color)value).a)} ]";
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
